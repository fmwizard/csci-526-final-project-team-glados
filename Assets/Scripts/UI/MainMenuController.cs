using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadBaseLevel()
    {
        SceneManager.LoadScene("lvl0");
        PlayerStats.levelNumber = 0;
        FirebaseManager.instance.LogLevelStart(0);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("lvl1");
        PlayerStats.levelNumber = 1;
        FirebaseManager.instance.LogLevelStart(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}