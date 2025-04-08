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

    public void LoadTutorialMenu()
    {
        LogMainMenuChoice("Tutorial Menu");
        SceneManager.LoadScene("TutorialMenu");
    }

    public void LoadTutorialLevel()
    {
        LogMainMenuChoice("Basic Tutorial");
        SceneManager.LoadScene("tutorial");
        PlayerStats.levelNumber = -1;
        FirebaseManager.instance.LogLevelStart(-1);
    }

    public void LoadAllyTutorialLevel()
    {
        LogMainMenuChoice("Ally Tutorial");
        SceneManager.LoadScene("allyTutorial");
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

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
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
        FirebaseManager.instance.SendDatabyPOST(path, json);
    }
}