using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1CompleteTrigger : MonoBehaviour
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
                FirebaseManager.instance.UpdateLevelCompletion(1, completionTime, deaths, retries);
            }

            SceneManager.LoadScene("Level1Complete");
        }
    }
}