using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static string playerID;
    public static int levelNumber = 0;
    public static int deathCount = 0;
    public static int retryCount = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (string.IsNullOrEmpty(playerID))
        {
            playerID = System.Guid.NewGuid().ToString();  // Generate Unique Player ID
        }
    }

    public static void IncreaseDeathCount()
    {
        deathCount++;
    }

    public static void IncreaseRetryCount()
    {
        retryCount++;
    }

    public static void ResetStats()
    {
        deathCount = 0;
        retryCount = 0;
    }
}