using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Level1CompleteUI : MonoBehaviour
{
    public TextMeshProUGUI timeText1;

    void Start()
    {
        float finalTime = PlayerPrefs.GetFloat("FinalTime", 0f);
        int minutes = Mathf.FloorToInt(finalTime / 60);
        int seconds = Mathf.FloorToInt(finalTime % 60);
        timeText1.text = "Time:\n" + string.Format("{0:00}:{1:00}", minutes, seconds);
        Debug.Log("Loaded Final Time: " + finalTime);
    }

    public void RetryLevel()
    {
        PlayerStats.IncreaseRetryCount();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        FirebaseManager.instance.UpdateDeathCount(1);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}