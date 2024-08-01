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

public class FirebaseController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtLogin;
    [SerializeField] private TextMeshProUGUI txtNickname;
    [SerializeField] private TMP_InputField inpNickname;
    [SerializeField] private GameObject createNickname;
    [SerializeField] private GameObject dlgNickname;
    [SerializeField] private Button btnStart;

    private FirebaseAuth auth; // 인증에 관한 정보 저장할 객체
    private FirebaseUser user; // 파이어베이스 유저의 정보를 담을 객체

    private string authCode;

    private Camera mainCamera;


    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(MoveCameraRoutine());

        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            if (success == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    authCode = code;

                    auth = FirebaseAuth.DefaultInstance;
                    Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

                    auth.SignInAndRetrieveDataWithCredentialAsync(credential)
                        .ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCompleted)
                            {
                                HasNicknameByID();
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
                if (snapshot.Exists)
                {
                    // 데이터가 존재하는 경우
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

        // <메세지 번호, 메세지 딕셔너리(유저이름,메세지내용)>
        Dictionary<string, object> nicknameDic = new Dictionary<string, object>();

        nicknameDic.Add(user.UserId, inpNickname.text);

        // chatDB에 updateMsg를 추가해서 데이터 업데이트
        nameDB.UpdateChildrenAsync(nicknameDic);

        dlgNickname.SetActive(false);
        createNickname.SetActive(false);
        txtLogin.gameObject.SetActive(true);

        StartGame();
    }

    public void ExitCreateNickname() // 다이얼로그 취소 버튼에 등록
    {
        dlgNickname.SetActive(false);
    }
    
}
