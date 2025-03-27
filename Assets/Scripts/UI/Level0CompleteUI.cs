using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Level0CompleteUI : MonoBehaviour
{
    public TextMeshProUGUI timeText0;

    void Start()
    {
        float finalTime = PlayerPrefs.GetFloat("FinalTime", 0f);
        int minutes = Mathf.FloorToInt(finalTime / 60);
        int seconds = Mathf.FloorToInt(finalTime % 60);
        timeText0.text = "Time:\n" + string.Format("{0:00}:{1:00}", minutes, seconds);
        Debug.Log("Loaded Final Time: " + finalTime);
    }

    public void RetryLevel()
    {
        PlayerStats.IncreaseRetryCount();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        FirebaseManager.instance.UpdateRetryCount(0);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        PlayerStats.levelNumber = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FirebaseManager.instance.LogLevelStart(1);
    }
}