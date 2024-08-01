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

    private FirebaseAuth auth; // ������ ���� ���� ������ ��ü
    private FirebaseUser user; // ���̾�̽� ������ ������ ���� ��ü

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
                if (snapshot.Exists)
                {
                    // �����Ͱ� �����ϴ� ���
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

        // <�޼��� ��ȣ, �޼��� ��ųʸ�(�����̸�,�޼�������)>
        Dictionary<string, object> nicknameDic = new Dictionary<string, object>();

        nicknameDic.Add(user.UserId, inpNickname.text);

        // chatDB�� updateMsg�� �߰��ؼ� ������ ������Ʈ
        nameDB.UpdateChildrenAsync(nicknameDic);

        dlgNickname.SetActive(false);
        createNickname.SetActive(false);
        txtLogin.gameObject.SetActive(true);

        StartGame();
    }

    public void ExitCreateNickname() // ���̾�α� ��� ��ư�� ���
    {
        dlgNickname.SetActive(false);
    }
    
}
