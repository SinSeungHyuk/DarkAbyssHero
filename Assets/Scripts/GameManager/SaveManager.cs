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
        // 플레이어의 PlayerSaveData 구조체를 Json 형태로 변환
        // PlayerSaveData 구조체 내부는 json으로 변환 가능한 int,List 등 기본자료형
        PlayerSaveData playerSaveData = player.ToSaveData();
        string saveData = JsonUtility.ToJson(playerSaveData);


        Dictionary<string, object> timeStampDic = new Dictionary<string, object>();
        timeStampDic.Add("TimeStamp", ServerValue.Timestamp);

        // SaveData 노드 아래에 user.UserId 자식을 생성해서 SetRawJsonValueAsync으로 데이터 저장
        databaseReference.Child("SaveData").Child(user.UserId).SetRawJsonValueAsync(saveData);
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
                    // 데이터를 찾아서 json 문자열로 변환
                    string json = snapshot.GetRawJsonValue();

                    // JSON을 JObject로 파싱
                    JObject jsonObject = JObject.Parse(json);

                    // 사용자 ID에 해당하는 내부 객체를 추출 (데이터에서 ID는 제외시키는 작업)
                    JToken userDataToken = jsonObject[user.UserId];

                    if (userDataToken != null)
                    {
                        // 내부 객체만을 다시 JSON 문자열로 변환
                        string userDataJson = userDataToken.ToString();

                        // 이제 이 JSON 문자열을 PlayerSaveData로 역직렬화
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
        saveDB.Child(user.UserId).Child("TimeStamp").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    long timeStamp = Convert.ToInt64(snapshot.Value);

                    long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    long timeDifferenceMs = currentTimestamp - timeStamp;

                    TimeSpan timeDifference = TimeSpan.FromMilliseconds(timeDifferenceMs);

                    // 시간 차이에 따른 보상 계산 및 지급
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
        // 모바일 게임은 일시정지(앱을 내릴때) 세이브
        if (pause)
            SaveGame();
    }
}
