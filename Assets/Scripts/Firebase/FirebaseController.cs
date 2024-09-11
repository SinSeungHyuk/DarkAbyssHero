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
    // ��ó�� �α��ξ��� ��ġ�Ǵ� �α��� ���� Ŭ����. ���̾�̽�+�����÷��̿� ����
    // �α��ΰ� UI�� ���� ����Ǿ��ִ� �κ��̱⿡ Ŭ������ ������ �ʰ� ���⿡�� ��� ����

    [SerializeField] private TextMeshProUGUI txtLogin;
    [SerializeField] private TextMeshProUGUI txtNickname;
    [SerializeField] private TMP_InputField inpNickname;
    [SerializeField] private GameObject createNickname;
    [SerializeField] private GameObject dlgNickname;
    [SerializeField] private Button btnStart;

    [SerializeField] private MusicTrackSO loginBGM;

    private FirebaseAuth auth; // ������ ���� ���� ������ ��ü
    private FirebaseUser user; // ���̾�̽� ������ ������ ���� ��ü

    private string authCode; // �α����� ���� �����ڵ�

    private Camera mainCamera;


    void Start()
    {
        // �α��ξ� BGM ���
        MusicManager.Instance.PlayMusic(loginBGM, 0f, 2f);

        mainCamera = Camera.main;
        StartCoroutine(MoveCameraRoutine());

        // �����÷��� �α���
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(success =>
        {
            if (success == SignInStatus.Success)
            {
                // RequestServerSideAccess : ServerAuthCode(= code) �� ��ȯ���ִ� �Լ� 
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    authCode = code;

                    // ������ ���� �ڵ带 �������� �α��� �������� �߱޹ޱ� (GetCredential)
                    auth = FirebaseAuth.DefaultInstance;
                    Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

                    auth.SignInAndRetrieveDataWithCredentialAsync(credential)
                        .ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCompleted)
                            {
                                HasNicknameByID(); // �α��ο� �����ϸ� ���� �ִ��� �˻�
                            }

                            Firebase.Auth.AuthResult result = task.Result;
                        });
                });
            }
        });
    }

    private IEnumerator MoveCameraRoutine()
    {
        float duration = 10f; // ȸ�� �ð�
        float[] angles = { -55.0f, -35.0f }; // ȸ���� ��ǥ �迭�� ����
        int currentIndex = 0; // ���� ȸ���� ��������� �ε����� üũ

        while (true)
        {
            float startAngle = angles[currentIndex];
            float goalAngle = angles[(currentIndex + 1) % 2];

            float elapsedTime = 0f;
            while (elapsedTime < duration) // �ð��� ���� ȸ������
            {
                // ��������~��ǥ�������� duration�� ���ļ� ȸ��
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float angle = Mathf.Lerp(startAngle, goalAngle, t); 

                // ���� ī�޶��� ȸ���� �޾ƿͼ� y�ุ ���ο� ȸ�������� ����
                Vector3 currentRotation = mainCamera.transform.rotation.eulerAngles;
                mainCamera.transform.rotation = Quaternion.Euler(currentRotation.x, angle, currentRotation.z);

                yield return null;
            }

            currentIndex = (currentIndex + 1) % 2; // ȸ�� ������� �ٲٱ�
        }
    }

    private void HasNicknameByID()
    {
        // ���� �α����� ���̾�̽� ������ UserId�� �����ͼ� Nickname �����Ͱ� �ִ��� �˻�

        user = auth.CurrentUser;
        DatabaseReference nameDB = FirebaseDatabase.DefaultInstance.GetReference("Nickname");

        nameDB.OrderByKey().EqualTo(user.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // ���� ó��
                Debug.Assert(task.Exception != null, "Task Exception!!"+ task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists) // �����Ͱ� �����ϴ� ��� (Exists = �����ϴ�)
                {
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        // �г��� ���ÿ� ���� (�ٸ� �������� ������ ���)
                        string nickname = childSnapshot.Value.ToString();
                        PlayerPrefs.SetString("Nickname", nickname);
                        PlayerPrefs.Save();

                        StartGame();
                        break; // ù ��° (�׸��� ������) �ڽĸ� ó���մϴ�.
                    }
                }
                else
                {
                    // �����Ͱ� �������� �ʴ� ���
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

    public void CreateDialogNickname() // Confirm ��ư�� ���
    {
        if (inpNickname.text == "") return;

        dlgNickname.SetActive(true);

        txtNickname.text = $"Use [{inpNickname.text}] ?";
    }

    public void CreateNickname() // ���̾�α� ��ư�� ���
    {
        DatabaseReference nameDB = FirebaseDatabase.DefaultInstance.GetReference("Nickname");

        // <�������̵�, �����г���> ��ųʸ� : ������ ID���� ������ �г��� ��������� ����
        Dictionary<string, object> nicknameDic = new Dictionary<string, object>();

        nicknameDic.Add(user.UserId, inpNickname.text);

        // nameDB�� nicknameDic�� �߰��ؼ� ������ ������Ʈ
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

    public void ExitCreateNickname() // ���̾�α� ��� ��ư�� ���
    {
        dlgNickname.SetActive(false);
    }
    
    public void LoadStartScene() // ��ŸƮ ��ư�� ���
    {
        // ���� ���࿡ �ʿ��� ���ҽ� �ε� (��巹���� Ȱ��)
        List<string> levelResources = new List<string> { "Database" , "Sprites" , "Prefabs"};
        LoadingSceneManager.LoadScene("MainScene", levelResources);
    }
}
