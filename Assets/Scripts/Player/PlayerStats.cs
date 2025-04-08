using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static string playerID;
    public static int levelNumber = 0;
    public static int levelCompleted = -10;
    public static Dictionary<int, int> retryCounts = new Dictionary<int, int>();
    public static Dictionary<int, int> deathCounts = new Dictionary<int, int>();
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (string.IsNullOrEmpty(playerID))
        {
            playerID = System.Guid.NewGuid().ToString();  // Generate Unique Player ID
        }
    }


    public static void ResetDeathCount(int level)
    {
        deathCounts[level] = 0;
    }

    public static void IncreaseDeathCount()
    {
        if (!deathCounts.ContainsKey(levelNumber))
            deathCounts[levelNumber] = 0;

        deathCounts[levelNumber]++;
    }

    public static void IncreaseRetryCount()
    {
        if (!retryCounts.ContainsKey(levelNumber))
            retryCounts[levelNumber] = 0;

        retryCounts[levelNumber]++;
    }

    public static int GetRetryCount(int level)
    {
        return retryCounts.ContainsKey(level) ? retryCounts[level] : 0;
    }

    public static int GetDeathCount(int level)
    {
        return deathCounts.ContainsKey(level) ? deathCounts[level] : 0;
    }

    public static void ResetStats()
    {
        retryCounts.Clear();
        deathCounts.Clear();
    }
}