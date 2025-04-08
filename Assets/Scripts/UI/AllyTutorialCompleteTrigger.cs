using UnityEngine;
using UnityEngine.SceneManagement;

public class AllyTutorialComplete : MonoBehaviour
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
            int deaths = PlayerStats.GetDeathCount(0);
            int retries = PlayerStats.GetRetryCount(0);

            if (FirebaseManager.instance != null)
            {
                FirebaseManager.instance.UpdateLevelCompletion(0, completionTime, deaths, retries);
            }

            SceneManager.LoadScene("AllyTutorialComplete");
        }
    }
}
