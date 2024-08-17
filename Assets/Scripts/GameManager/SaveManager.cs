using UnityEngine;
using System.Collections.Generic;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;

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

    public async Task SaveGame()
    {
        string saveData = JsonUtility.ToJson(player.ToSaveData());
        await databaseReference.Child("SaveData").Child(user.UserId).SetRawJsonValueAsync(saveData);
    }

    public void LoadGame()
    {
        DatabaseReference saveDB = FirebaseDatabase.DefaultInstance.GetReference("SaveData");

        saveDB.OrderByKey().EqualTo(user.UserId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                // 에러 처리
                Debug.Assert(task.Exception != null, "Task Exception!!" + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    // 데이터가 존재하는 경우
                    string json = snapshot.GetRawJsonValue();
                    var saveData = JsonUtility.FromJson<PlayerSaveData>(json);
                    player.FromSaveData(saveData);
                }
            }
        });
    }

    private void OnApplicationQuit()
    {
        SaveGame().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to save game on quit: {task.Exception}");
            }
        });
    }
}
