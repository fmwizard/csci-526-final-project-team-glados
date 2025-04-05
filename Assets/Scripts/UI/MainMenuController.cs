using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        InitializeControlsAnalytics();
    }

    private void InitializeControlsAnalytics()
    {
        string playerID = PlayerStats.playerID;
        string path = $"test/{playerID}/MainMenu/controlsViewed";
        string json = "{\"opened\": false}";
        FirebaseManager.instance.SendDatabyPUT(path, json);
    }

    public void LoadTutorialLevel()
    {
        LogMainMenuChoice("Tutorial");
        SceneManager.LoadScene("tutorial");
        PlayerStats.levelNumber = 0;
        FirebaseManager.instance.LogLevelStart(0);
    }

    public void LoadLevel1()
    {
        LogMainMenuChoice("Level 1");
        SceneManager.LoadScene("lvl1");
        PlayerStats.levelNumber = 1;
        FirebaseManager.instance.LogLevelStart(1);
    }

    public void LoadLevel2()
    {
        LogMainMenuChoice("Level 2");
        SceneManager.LoadScene("lvl2");
        PlayerStats.levelNumber = 2;
        FirebaseManager.instance.LogLevelStart(2);
    }

    public void OnControlsButtonClicked()
    {
        string playerID = PlayerStats.playerID;
        string path = $"test/{playerID}/MainMenu/controlsViewed";
        string json = "{\"opened\": true}";
        FirebaseManager.instance.SendDatabyPUT(path, json);

        // Show controls panel or scene if needed
    }

    private void LogMainMenuChoice(string choice)
    {
        string playerID = PlayerStats.playerID;
        string path = $"test/{playerID}/MainMenu/selectedOption";
        string json = $"{{\"choice\": \"{choice}\", \"timestamp\": {Time.time}}}";
        FirebaseManager.instance.SendDatabyPUT(path, json);
    }
}