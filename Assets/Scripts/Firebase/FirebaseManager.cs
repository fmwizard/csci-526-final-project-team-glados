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

    // Logs Level Completion with all player stats
    public void LogLevelCompletion(int levelNumber, float completionTime, int deaths, int retries, bool completed)
    {
        string playerID = PlayerStats.playerID;
        string path = $"levelCompletion/{playerID}/level_{levelNumber}";

        string json = $"{{\"completionTime\": \"{(completed ? completionTime.ToString() : "N/A")}\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": {completed.ToString().ToLower()}}}";
        SendData(path, json);

        PlayerStats.ResetStats();
    }

    // Logs if a player quits mid-level
    public void LogIncompleteSession()
    {
        string playerID = PlayerStats.playerID;
        string path = $"levelCompletion/{playerID}/level_0/1";

        string json = $"{{\"completionTime\": \"N/A\", \"deaths\": {PlayerStats.deathCount}, \"retries\": {PlayerStats.retryCount}, \"completed\": false}}";
        SendData(path, json);

        PlayerStats.ResetStats(); 
    }
}