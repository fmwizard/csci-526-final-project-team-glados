using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private string playerID;

    void Start()
    {
        playerID = PlayerStats.playerID;
        InitializeControlsAnalytics();
    }

    private void InitializeControlsAnalytics()
    {
        string path = $"test/{playerID}/MainMenu/controlsViewed";
        string json = "{\"opened\": false}";
        if (FirebaseManager.instance != null)
        {
            FirebaseManager.instance.SendDatabyPUT(path, json);
        }    
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
        string path = $"test/{playerID}/MainMenu/controlsViewed";
        string json = "{\"opened\": true}";
        if (FirebaseManager.instance != null)
        {
            FirebaseManager.instance.SendDatabyPUT(path, json);
        }
    }

    private void LogMainMenuChoice(string choice)
    {
        string path = $"test/{playerID}/MainMenu/selectedOption";
        string json = $"{{\"choice\": \"{choice}\", \"timestamp\": {Time.time}}}";
        FirebaseManager.instance.SendDatabyPUT(path, json);
    }
}