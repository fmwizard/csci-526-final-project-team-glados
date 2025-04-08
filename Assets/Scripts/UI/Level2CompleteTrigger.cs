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
            int deaths = PlayerStats.GetDeathCount(2);
            int retries = PlayerStats.GetRetryCount(2);

            if (FirebaseManager.instance != null)
            {
                FirebaseManager.instance.UpdateLevelCompletion(2, completionTime, deaths, retries);
            }

            SceneManager.LoadScene("Level2Complete");
        }
    }
}