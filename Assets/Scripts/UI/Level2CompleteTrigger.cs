using UnityEngine;
using UnityEngine.SceneManagement;

public class Level2CompleteTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelTimer timer = FindObjectOfType<LevelTimer>();
            if (timer != null)
            {
                timer.StopTimer();
            }

            float completionTime = timer != null ? timer.GetTime() : 0f;
            int deaths = PlayerStats.deathCount;
            int retries = PlayerStats.retryCount;

            if (FirebaseManager.instance != null)
            {
                FirebaseManager.instance.UpdateLevelCompletion(2, completionTime, deaths, retries);
            }
            PlayerStats.levelCompleted = 2;

            SceneManager.LoadScene("Level2Complete");
        }
    }
}