using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class FirebaseManager : MonoBehaviour
{
    private string databaseURL = "https://portalmario-cs526-default-rtdb.firebaseio.com/";
    public static FirebaseManager instance;
    public bool enableLogging = false;
    public bool allowLoggingInEditor = true; // Test-only flag

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Ensures this object persists across scenes
        }
        else
        {
            Destroy(gameObject); // Prevents duplicates
        }
    }

    public void SendDatabyPUT(string path, string json)
    {
        if (!allowLoggingInEditor && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("Skipping Firebase Logging: Running in Unity Editor or Standalone.");
            return;
        }

        StartCoroutine(PutToDatabase(path, json));
    }
    
    public void SendDatabyPOST(string path, string json)
    {
        if (!allowLoggingInEditor && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("Skipping Firebase Logging: Running in Unity Editor or Standalone.");
            return;
        }

        StartCoroutine(PostToDatabase(path, json));
    }

    IEnumerator PutToDatabase(string path, string json)
    {
        string fullURL = databaseURL + path + ".json";
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(fullURL, "PUT");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data successfully sent to Firebase!");
        }
        else
        {
            Debug.LogError("Error sending data: " + request.error);
        }
    }

    IEnumerator PostToDatabase(string path, string json)
    {
        string fullURL = databaseURL + path + ".json";
        byte[] jsonToSend = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(fullURL, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data successfully sent to Firebase!");
        }
        else
        {
            Debug.LogError("Error sending data: " + request.error);
        }
    }

    public void LogLevelStart(int levelNumber)
    {
        string playerID = PlayerStats.playerID;
        string path = $"testCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"N/A\", \"deaths\": 0, \"retries\": 0, \"completed\": false}}";
        SendDatabyPUT(path, json);
    }

    public void LogTestDatabyPOST(string key, object data, int levelNumber)
    {
        string playerID = PlayerStats.playerID;
        string path = $"test/{playerID}/level_{levelNumber}/{key}";
        string json = JsonUtility.ToJson(data);
        SendDatabyPOST(path, json);
    }

    public void UpdateDeathCount(int levelNumber)
    {
        string playerID = PlayerStats.playerID;
        string path = $"testCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"N/A\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": false}}";
        SendDatabyPUT(path, json);
    }

    public void UpdateRetryCount(int levelNumber)
    {
        string playerID = PlayerStats.playerID;
        string path = $"testCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"N/A\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": false}}";
        SendDatabyPUT(path, json);
    }

    public void UpdateLevelCompletion(int levelNumber, float completionTime, int deaths, int retries)
    {
        string playerID = PlayerStats.playerID;
        string path = $"testCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"{completionTime}\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": true}}";
        SendDatabyPUT(path, json);

        PlayerStats.ResetStats();
    }

    // For spacebar press logging
    [System.Serializable]
    public class KeyPressData
    {
        public float x;
        public float y;
        public float time;
    }

    public void LogKeyPress(Vector2 position, float time, int level)
    {
        KeyPressData data = new KeyPressData {
            x = position.x,
            y = position.y,
            time = time,
        };

        LogTestDatabyPOST("keyPresses", data, level);
    }

    public void LogEnemyKill(string reason, Vector2 position, int level)
    {
        EnemyKillData data = new EnemyKillData(reason, position, Time.timeSinceLevelLoad);
        LogTestDatabyPOST("enemy_kills", data, level);
    }
}