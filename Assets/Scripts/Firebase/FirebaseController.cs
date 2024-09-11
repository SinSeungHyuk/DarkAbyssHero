using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Auth;
using System.Threading.Tasks;
using System;
using TMPro;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using Google;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FirebaseController : MonoBehaviour
{
    // 맨처음 로그인씬에 배치되는 로그인 관리 클래스. 파이어베이스+구글플레이와 연동
    // 로그인과 UI는 같이 연결되어있는 부분이기에 클래스를 나누지 않고 여기에서 모두 관리

    [SerializeField] private TextMeshProUGUI txtLogin;
    [SerializeField] private TextMeshProUGUI txtNickname;
    [SerializeField] private TMP_InputField inpNickname;
    [SerializeField] private GameObject createNickname;
    [SerializeField] private GameObject dlgNickname;
    [SerializeField] private Button btnStart;

    [SerializeField] private MusicTrackSO loginBGM;

    private FirebaseAuth auth; // 인증에 관한 정보 저장할 객체
    private FirebaseUser user; // 파이어베이스 유저의 정보를 담을 객체

    private string authCode; // 로그인을 위한 유저코드

    private Camera mainCamera;


    void Start()
    {
        // 로그인씬 BGM 재생
        MusicManager.Instance.PlayMusic(loginBGM, 0f, 2f);

        mainCamera = Camera.main;
        StartCoroutine(MoveCameraRoutine());

        // 구글플레이 로그인
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            if (success == SignInStatus.Success)
            {
                // RequestServerSideAccess : ServerAuthCode(= code) 를 반환해주는 함수 
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    authCode = code;

                    // 위에서 받은 코드를 바탕으로 로그인 인증서를 발급받기 (GetCredential)
                    auth = FirebaseAuth.DefaultInstance;
                    Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

                    auth.SignInAndRetrieveDataWithCredentialAsync(credential)
                        .ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCompleted)
                            {
                                HasNicknameByID(); // 로그인에 성공하면 계정 있는지 검사
                            }

                            Firebase.Auth.AuthResult result = task.Result;
                        });
                });
            }
        });
    }

    private IEnumerator MoveCameraRoutine()
    {
        float duration = 10f; // 회전 시간
        float[] angles = { -55.0f, -35.0f }; // 회전할 목표 배열로 관리
        int currentIndex = 0; // 현재 회전의 진행방향을 인덱스로 체크

        while (true)
        {
            float startAngle = angles[currentIndex];
            float goalAngle = angles[(currentIndex + 1) % 2];

            float elapsedTime = 0f;
            while (elapsedTime < duration) // 시간을 통해 회전관리
            {
                // 시작지점~목표지점까지 duration에 걸쳐서 회전
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float angle = Mathf.Lerp(startAngle, goalAngle, t); 

                // 현재 카메라의 회전값 받아와서 y축만 새로운 회전값으로 설정
                Vector3 currentRotation = mainCamera.transform.rotation.eulerAngles;
                mainCamera.transform.rotation = Quaternion.Euler(currentRotation.x, angle, currentRotation.z);

                yield return null;
            }

            currentIndex = (currentIndex + 1) % 2; // 회전 진행방향 바꾸기
        }
    }

    private void HasNicknameByID()
    {
        // 현재 로그인한 파이어베이스 계정의 UserId를 가져와서 Nickname 데이터가 있는지 검사

        user = auth.CurrentUser;
        DatabaseReference nameDB = FirebaseDatabase.DefaultInstance.GetReference("Nickname");

        nameDB.OrderByKey().EqualTo(user.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // 에러 처리
                Debug.Assert(task.Exception != null, "Task Exception!!"+ task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists) // 데이터가 존재하는 경우 (Exists = 존재하다)
                {
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        // 닉네임 로컬에 저장 (다른 씬에서도 빠르게 사용)
                        string nickname = childSnapshot.Value.ToString();
                        PlayerPrefs.SetString("Nickname", nickname);
                        PlayerPrefs.Save();

                        StartGame();
                        break; // 첫 번째 (그리고 유일한) 자식만 처리합니다.
                    }
                }
                else
                {
                    // 데이터가 존재하지 않는 경우
                    txtLogin.gameObject.SetActive(false);
                    createNickname.SetActive(true);
                }
            }
        });
    }

    private void StartGame()
    {
        btnStart.gameObject.SetActive(true);
        txtLogin.text = "Touch To Start";
    }

    public void CreateDialogNickname() // Confirm 버튼에 등록
    {
        if (inpNickname.text == "") return;

        dlgNickname.SetActive(true);

        txtNickname.text = $"Use [{inpNickname.text}] ?";
    }

    public void CreateNickname() // 다이얼로그 버튼에 등록
    {
        DatabaseReference nameDB = FirebaseDatabase.DefaultInstance.GetReference("Nickname");

        // <유저아이디, 유저닉네임> 딕셔너리 : 유저의 ID마다 고유한 닉네임 밸류값으로 저장
        Dictionary<string, object> nicknameDic = new Dictionary<string, object>();

        nicknameDic.Add(user.UserId, inpNickname.text);

        // nameDB에 nicknameDic를 추가해서 데이터 업데이트
        nameDB.UpdateChildrenAsync(nicknameDic).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled) dlgNickname.SetActive(false); 

            else if (task.IsCompleted)
            {
                dlgNickname.SetActive(false);
                createNickname.SetActive(false);
                txtLogin.gameObject.SetActive(true);

                StartGame();
            }
        });
    }

    public void ExitCreateNickname() // 다이얼로그 취소 버튼에 등록
    {
        dlgNickname.SetActive(false);
    }
    
    public void LoadStartScene() // 스타트 버튼에 등록
    {
        // 최초 실행에 필요한 리소스 로드 (어드레서블 활용)
        List<string> levelResources = new List<string> { "Database" , "Sprites" , "Prefabs"};
        LoadingSceneManager.LoadScene("MainScene", levelResources);
    }
}
