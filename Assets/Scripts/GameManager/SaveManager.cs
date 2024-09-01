using UnityEngine;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


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
        string saveData = JsonUtility.ToJson(player.ToSaveData());

        // SaveData ��� �Ʒ��� user.UserId �ڽ��� �����ؼ� SetRawJsonValueAsync���� ������ ����
        databaseReference.Child("SaveData").Child(user.UserId).SetRawJsonValueAsync(saveData);
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
                    }
                    else Debug.Log("User ID no found");
                }
                else Debug.Log("no save data");
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
