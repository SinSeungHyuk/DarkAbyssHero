using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;



public class ChatUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inpMessage;

    [SerializeField] private MessageBox msgBoxPrefab;
    [SerializeField] private Transform msgBoxContents;

    private string nickname;

    private List<string> receiveKeyList = new List<string>();


    void Start()
    {
        nickname = PlayerPrefs.GetString("Nickname"); // 로컬에 저장된 닉네임 가져오기

        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");
        // 타임스탬프를 사용하여 정렬한 이후 하나의 값을 가져오기
        chatDB.OrderByChild("timestamp").LimitToLast(1).ValueChanged += ReceiveMessage;
    }

    public void ReceiveMessage(object sender, ValueChangedEventArgs e)
    {
        DataSnapshot snapshot = e.Snapshot;

        foreach (var data in snapshot.Children) // ChatMessage 노드 안의 메세지들
        {
            // 이미 입력된 채팅이 중복되어서 처리되는 상황을 방지하기 위한 안전장치
            if (receiveKeyList.Contains(data.Key)) continue;

            string username = data.Child("username").Value.ToString();
            string msg = data.Child("message").Value.ToString();
            AddChatMessage(username, msg);
            receiveKeyList.Add(data.Key);
        }
    }

    public void SendChatMessage()
    {
        string message = inpMessage.text;

        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");

        // 새 메세지를 생성하기 위해 키값을 생성하기 (Push().Key로 자동생성)
        string key = chatDB.Push().Key;

        // <메세지 번호(key), 메세지 딕셔너리(유저이름,메세지내용)>
        Dictionary<string, object> updateMsg = new Dictionary<string, object>();
        // 유저네임,메세지,타임스탬프가 들어있는 딕셔너리
        Dictionary<string, object> msgDic = new Dictionary<string, object>();
        msgDic.Add("message", message);
        msgDic.Add("username", nickname);
        msgDic.Add("timestamp", ServerValue.Timestamp);

        updateMsg.Add(key, msgDic); // 메세지번호,메세지를 키-밸류로 딕셔너리에 삽입

        // chatDB에 updateMsg를 추가해서 데이터 업데이트
        chatDB.UpdateChildrenAsync(updateMsg)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    inpMessage.text = "";
            });
    }

    private void AddChatMessage(string username, string msg)
    {
        // msgBoxContents의 자식오브젝트로 프리팹 생성
        MessageBox msgBox = Instantiate(msgBoxPrefab, msgBoxContents);

        msgBox.SetMessage(username, msg);
    }
}
