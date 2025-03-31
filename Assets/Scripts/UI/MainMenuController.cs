using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadTutorialLevel()
    {
        SceneManager.LoadScene("tutorial");
        PlayerStats.levelNumber = 0;
        FirebaseManager.instance.LogLevelStart(0);
    }
    
    public void LoadLevel1()
    {
        SceneManager.LoadScene("lvl1");
        PlayerStats.levelNumber = 1;
        FirebaseManager.instance.LogLevelStart(1);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("lvl2");
        PlayerStats.levelNumber = 2;
        FirebaseManager.instance.LogLevelStart(2);
    }

    // public void QuitGame()
    // {
    //     Application.Quit();
    // }
}