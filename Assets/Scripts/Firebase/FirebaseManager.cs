using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class FirebaseManager : MonoBehaviour
{
    private string databaseURL = "https://portalmario-cs526-default-rtdb.firebaseio.com/";
    public static FirebaseManager instance;
    public bool enableLogging = false;

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

    public void SendData(string path, string json)
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            Debug.Log("Skipping Firebase Logging: Running in Unity Editor or Standalone.");
            return;
        }

        StartCoroutine(PostToDatabase(path, json));
    }

    IEnumerator PostToDatabase(string path, string json)
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

    public void LogLevelStart(int levelNumber)
    {
        string playerID = PlayerStats.playerID;
        string path = $"levelCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"N/A\", \"deaths\": 0, \"retries\": 0, \"completed\": false}}";
        SendData(path, json);
    }

    public void UpdateDeathCount(int levelNumber)
    {
        string playerID = PlayerStats.playerID;
        string path = $"levelCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"N/A\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": false}}";
        SendData(path, json);
    }

    public void UpdateRetryCount(int levelNumber)
    {
        string playerID = PlayerStats.playerID;
        string path = $"levelCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"N/A\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": false}}";
        SendData(path, json);
    }

    public void UpdateLevelCompletion(int levelNumber, float completionTime, int deaths, int retries)
    {
        string playerID = PlayerStats.playerID;
        string path = $"levelCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"{completionTime}\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": true}}";
        SendData(path, json);

        PlayerStats.ResetStats();
    }
}