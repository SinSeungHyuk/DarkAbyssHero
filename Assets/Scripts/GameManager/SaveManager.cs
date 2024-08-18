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
        string saveData = JsonUtility.ToJson(player.ToSaveData());
        databaseReference.Child("SaveData").Child(user.UserId).SetRawJsonValueAsync(saveData);
    }

    public void LoadGame()
    {
        DatabaseReference saveDB = FirebaseDatabase.DefaultInstance.GetReference("SaveData");
        saveDB.OrderByKey().EqualTo(user.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Task Exception: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    Debug.Log("Loaded JSON: " + json);

                    // JSON�� JObject�� �Ľ�
                    JObject jsonObject = JObject.Parse(json);

                    // ����� ID�� �ش��ϴ� ���� ��ü�� ����
                    JToken userDataToken = jsonObject[user.UserId];

                    if (userDataToken != null)
                    {
                        // ���� ��ü���� �ٽ� JSON ���ڿ��� ��ȯ
                        string userDataJson = userDataToken.ToString();

                        // ���� �� JSON ���ڿ��� PlayerSaveData�� ������ȭ
                        var saveData = JsonConvert.DeserializeObject<PlayerSaveData>(userDataJson);

                        Debug.Log("Deserialized exp: " + saveData.LevelData.exp);
                        player.FromSaveData(saveData);
                    }
                    else
                    {
                        Debug.Log("No save data found for this user ID.");
                    }
                }
                else
                {
                    Debug.Log("No save data found for this user.");
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
        if (pause)
            SaveGame();
    }
}
