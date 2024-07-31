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
    public TextMeshProUGUI TxtFirebase;
    public TextMeshProUGUI TxtGoogle;
    public Button BtnCreateNickname;
    public TMP_InputField InpNickname;

    private string nickname;

    private FirebaseAuth auth; // ������ ���� ���� ������ ��ü
    private FirebaseUser user; // ���̾�̽� ������ ������ ���� ��ü

    private string authCode;

    private List<string> receiveKeyList = new List<string>();


    void Start()
    {
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
                                testc();
                            }

                            Firebase.Auth.AuthResult result = task.Result;
                        });
                });
            }
        });
    }

    private void testc()
    {
        TxtGoogle.text = "success";
        TxtFirebase.text = auth.CurrentUser.DisplayName; // �÷��̰����� �г���

        user = auth.CurrentUser;

        BtnCreateNickname.interactable = true;

        HasNicknameByID();
    }

    private void HasNicknameByID()
    {
        DatabaseReference nameDB = FirebaseDatabase.DefaultInstance.GetReference("Nickname");

        nameDB.OrderByKey().EqualTo(user.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // ���� ó��
                TxtGoogle.text = "No nickname";
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // �����Ͱ� �����ϴ� ���
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        nickname = childSnapshot.Value.ToString();
                        SendChatMessage("Test Message!!!");
                        break; // ù ��° (�׸��� ������) �ڽĸ� ó���մϴ�.
                    }
                }
                else
                {
                    // �����Ͱ� �������� �ʴ� ���
                    Debug.Log("User does not exist: ");
                }
            }
        });
    }

    public void SendChatMessage(string msg)
    {
        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");

        // �� �޼����� �����ϱ� ���� Ű���� �����ϱ� (Push().Key�� �ڵ�����)
        string key = chatDB.Push().Key;

        // <�޼��� ��ȣ, �޼��� ��ųʸ�(�����̸�,�޼�������)>
        Dictionary<string, object> updateMsg = new Dictionary<string, object>();
        // ��������,�޼����� ����ִ� ��ųʸ�
        Dictionary<string, object> msgDic = new Dictionary<string, object>();
        msgDic.Add("message", msg);
        msgDic.Add("username", nickname);

        msgDic.Add("timestamp", ServerValue.Timestamp);

        updateMsg.Add(key, msgDic); // �޼�����ȣ,�޼����� Ű-����� ��ųʸ��� ����

        // chatDB�� updateMsg�� �߰��ؼ� ������ ������Ʈ
        chatDB.UpdateChildrenAsync(updateMsg)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log(key);
            });
    }

    public void CreateNickname()
    {
        if (InpNickname.text == "") return;

        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("Nickname");

        // <�޼��� ��ȣ, �޼��� ��ųʸ�(�����̸�,�޼�������)>
        Dictionary<string, object> nicknameDic = new Dictionary<string, object>();

        nicknameDic.Add(user.UserId, InpNickname.text);

        // chatDB�� updateMsg�� �߰��ؼ� ������ ������Ʈ
        chatDB.UpdateChildrenAsync(nicknameDic);
    }


    //private void ProcessAuthentication(bool success)
    //{
    //    if (success)
    //    {
    //        TxtGoogle.text = "success";
    //        string authCode = null;

    //        //string authCode = PlayGamesPlatform.Instance.GetServerAuthCode();

    //        PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
    //        {
    //            authCode = code;
    //            Debug.Log(code + " -------------------------------------- collduty");
    //        });

    //        //Firebase.Auth.Credential credential = PlayGamesAuthProvider.GetCredential(authCode);

    //        auth.SignInAndRetrieveDataWithCredentialAsync(authCode)
    //            .ContinueWithOnMainThread(task =>
    //            {
    //                Debug.Log(credential.IsValid() + " -------------------------------------- cdtty");

    //                if (task.IsCompleted)
    //                {
    //                    Debug.Log("************************ firebase login ************************");
    //                    AuthResult result = task.Result;
    //                }
    //                else if (task.IsFaulted)
    //                {
    //                    Debug.LogError(task.Exception);
    //                    return;
    //                }
    //            });
    //    }

    //    else TxtGoogle.text = "failed";
    //}

    //private void FirebaseInit()
    //{
    //    auth = FirebaseAuth.DefaultInstance;

    //    // auth ��ü�� �̺�Ʈ�� �Լ� ����
    //    auth.StateChanged += AuthStateChanged;
    //}

    //private void AuthStateChanged(object sender, EventArgs args)
    //{
    //    FirebaseAuth senderAuth = sender as FirebaseAuth;

    //    if (senderAuth != null)
    //    {
    //        // CurrentUser : ���� ������ ������ ����
    //        user = senderAuth.CurrentUser;
    //        if (user != null)
    //        {
    //            Debug.Log(user.UserId); // ������ �ִٸ� id �α�
    //            TxtFirebase.text = user.UserId + "\n" + user.DisplayName;
    //        }
    //    }
    //}

    //public void SignIn()
    //{
    //    SignInAnonymous();
    //}
    //private Task SignInAnonymous()
    //{
    //    return auth.SignInAnonymouslyAsync().
    //        ContinueWithOnMainThread(task =>
    //        {
    //            if (task.IsFaulted) Debug.LogError("SignIn Fail!!!!");
    //            else if (task.IsCompleted) Debug.Log("SignIn Completed");
    //        });
    //}
}
