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
        nickname = PlayerPrefs.GetString("Nickname"); // ���ÿ� ����� �г��� ��������

        DatabaseReference chatDB = FirebaseDatabase.DefaultInstance.GetReference("ChatMessage");
        // Ÿ�ӽ������� ����Ͽ� ������ ���� �ϳ��� ���� ��������
        chatDB.OrderByChild("timestamp").LimitToLast(1).ValueChanged += ReceiveMessage;
    }

    public void ReceiveMessage(object sender, ValueChangedEventArgs e)
    {
        DataSnapshot snapshot = e.Snapshot;

        foreach (var data in snapshot.Children) // ChatMessage ��� ���� �޼�����
        {
            // �̹� �Էµ� ä���� �ߺ��Ǿ ó���Ǵ� ��Ȳ�� �����ϱ� ���� ������ġ
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

        // �� �޼����� �����ϱ� ���� Ű���� �����ϱ� (Push().Key�� �ڵ�����)
        string key = chatDB.Push().Key;

        // <�޼��� ��ȣ(key), �޼��� ��ųʸ�(�����̸�,�޼�������)>
        Dictionary<string, object> updateMsg = new Dictionary<string, object>();
        // ��������,�޼���,Ÿ�ӽ������� ����ִ� ��ųʸ�
        Dictionary<string, object> msgDic = new Dictionary<string, object>();
        msgDic.Add("message", message);
        msgDic.Add("username", nickname);
        msgDic.Add("timestamp", ServerValue.Timestamp);

        updateMsg.Add(key, msgDic); // �޼�����ȣ,�޼����� Ű-����� ��ųʸ��� ����

        // chatDB�� updateMsg�� �߰��ؼ� ������ ������Ʈ
        chatDB.UpdateChildrenAsync(updateMsg)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    inpMessage.text = "";
            });
    }

    private void AddChatMessage(string username, string msg)
    {
        // msgBoxContents�� �ڽĿ�����Ʈ�� ������ ����
        MessageBox msgBox = Instantiate(msgBoxPrefab, msgBoxContents);

        msgBox.SetMessage(username, msg);
    }
}
