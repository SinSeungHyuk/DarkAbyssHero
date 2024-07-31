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

    private FirebaseAuth auth; // 인증에 관한 정보 저장할 객체
    private FirebaseUser user; // 파이어베이스 유저의 정보를 담을 객체

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
        TxtFirebase.text = auth.CurrentUser.DisplayName; // 플레이게임즈 닉네임

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
                // 에러 처리
                TxtGoogle.text = "No nickname";
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // 데이터가 존재하는 경우
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        nickname = childSnapshot.Value.ToString();
                        SendChatMessage("Test Message!!!");
                        break; // 첫 번째 (그리고 유일한) 자식만 처리합니다.
                    }
                }
                else
                {
                    // 데이터가 존재하지 않는 경우
                    Debug.Log("User does not exist: ");
                }
            }
        });
    }

    public void SendChatMessage(string msg)
    {
        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");

        // 새 메세지를 생성하기 위해 키값을 생성하기 (Push().Key로 자동생성)
        string key = chatDB.Push().Key;

        // <메세지 번호, 메세지 딕셔너리(유저이름,메세지내용)>
        Dictionary<string, object> updateMsg = new Dictionary<string, object>();
        // 유저네임,메세지가 들어있는 딕셔너리
        Dictionary<string, object> msgDic = new Dictionary<string, object>();
        msgDic.Add("message", msg);
        msgDic.Add("username", nickname);

        msgDic.Add("timestamp", ServerValue.Timestamp);

        updateMsg.Add(key, msgDic); // 메세지번호,메세지를 키-밸류로 딕셔너리에 삽입

        // chatDB에 updateMsg를 추가해서 데이터 업데이트
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

        // <메세지 번호, 메세지 딕셔너리(유저이름,메세지내용)>
        Dictionary<string, object> nicknameDic = new Dictionary<string, object>();

        nicknameDic.Add(user.UserId, InpNickname.text);

        // chatDB에 updateMsg를 추가해서 데이터 업데이트
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

    //    // auth 객체의 이벤트에 함수 구독
    //    auth.StateChanged += AuthStateChanged;
    //}

    //private void AuthStateChanged(object sender, EventArgs args)
    //{
    //    FirebaseAuth senderAuth = sender as FirebaseAuth;

    //    if (senderAuth != null)
    //    {
    //        // CurrentUser : 현재 접속한 유저의 정보
    //        user = senderAuth.CurrentUser;
    //        if (user != null)
    //        {
    //            Debug.Log(user.UserId); // 유저가 있다면 id 로그
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
