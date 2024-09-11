using UnityEngine;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.Remoting.Messaging;
using System;


public class SaveManager : Singleton<SaveManager>
{
    private Player player;
    private DatabaseReference databaseReference;
    private FirebaseUser user;


    private void Start()
    {
        player = GameManager.Instance.GetPlayer();
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }

    public void SaveGame()
    {
        // �÷��̾��� PlayerSaveData ����ü�� Json ���·� ��ȯ
        // PlayerSaveData ����ü ���δ� json���� ��ȯ ������ int,List �� �⺻�ڷ���
        PlayerSaveData playerSaveData = player.ToSaveData();
        string saveData = JsonUtility.ToJson(playerSaveData);

        // ������ ���ӽð��� �����ϱ� ���� Ÿ�ӽ����� ��ųʸ��� ���� ����
        Dictionary<string, object> timeStampDic = new Dictionary<string, object>();
        timeStampDic.Add("TimeStamp", ServerValue.Timestamp);

        // SaveData ��� �Ʒ��� user.UserId �ڽ��� �����ؼ� SetRawJsonValueAsync���� ������ ����
        databaseReference.Child("SaveData").Child(user.UserId).SetRawJsonValueAsync(saveData);
        // Ÿ�ӽ������� �ش� ��忡 UpdateChildrenAsync �Լ��� �����͸� ���� �߰��Ͽ� ������Ʈ
        databaseReference.Child("SaveData").Child(user.UserId).UpdateChildrenAsync(timeStampDic);
    }

    public void LoadGame()
    {
        DatabaseReference saveDB = FirebaseDatabase.DefaultInstance.GetReference("SaveData");
        saveDB.OrderByKey().EqualTo(user.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted) 
                Debug.LogError("Task Exception: " + task.Exception);
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // �����͸� ã�Ƽ� json ���ڿ��� ��ȯ
                    string json = snapshot.GetRawJsonValue();

                    // JSON�� JObject�� �Ľ�
                    JObject jsonObject = JObject.Parse(json);

                    // ����� ID�� �ش��ϴ� ���� ��ü�� ���� (�����Ϳ��� ID�� ���ܽ�Ű�� �۾�)
                    JToken userDataToken = jsonObject[user.UserId];

                    if (userDataToken != null)
                    {
                        // ���� ��ü���� �ٽ� JSON ���ڿ��� ��ȯ
                        string userDataJson = userDataToken.ToString();

                        // ���� �� JSON ���ڿ��� PlayerSaveData�� ������ȭ
                        var saveData = JsonConvert.DeserializeObject<PlayerSaveData>(userDataJson);

                        player.FromSaveData(saveData);
                        GetReward(saveDB);
                    }
                    else Debug.Log("User ID no found");
                }
                else Debug.Log("no save data");
            }
        });
    }

    private void GetReward(DatabaseReference saveDB)
    {
        // �����͸� �ε��ϴ� �������� ���ӽð��� ���� ������ ���

        saveDB.Child(user.UserId).Child("TimeStamp").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    long timeStamp = Convert.ToInt64(snapshot.Value); // TimeStamp �����͸� long���� ��ȯ

                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // ����ð�
                    long timeDifferenceMs = currentTimestamp - timeStamp;

                    // (����ð� - ������ ���ӽð�)�� ���ؼ� TimeSpan Ÿ������ ��ȯ
                    TimeSpan timeDifference = TimeSpan.FromMilliseconds(timeDifferenceMs);

                    // �ð� ���̿� ���� ���� ��� �� ����
                    GameManager.Instance.GiveReward(timeDifference.TotalHours);
                }
            }
        });
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
        // ����� ������ �Ͻ�����(���� ������) ���̺�
        if (pause)
            SaveGame();
    }
}
