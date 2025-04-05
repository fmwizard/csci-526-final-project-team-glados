using UnityEngine;

[System.Serializable]
public class JumpEventData
{
    public float posX;
    public float posY;
    public float timestamp;

    public JumpEventData(Vector2 position, float time)
    {
        posX = position.x;
        posY = position.y;
        timestamp = time;
    }
}

[System.Serializable]
public class MirrorUseEvent
{
    public float posX;
    public float posY;
    public float timestamp;

    public MirrorUseEvent(Vector2 position, float time)
    {
        posX = position.x;
        posY = position.y;
        timestamp = time;
    }
}

[System.Serializable]
public class CatchUseEvent
{
    public float posX;
    public float posY;
    public float timestamp;

    public CatchUseEvent(Vector2 position, float time)
    {
        posX = position.x;
        posY = position.y;
        timestamp = time;
    }
}

[System.Serializable]
public class ReleaseUseEvent
{
    public float posX;
    public float posY;
    public float timestamp;

    public ReleaseUseEvent(Vector2 position, float time)
    {
        posX = position.x;
        posY = position.y;
        timestamp = time;
    }
}

[System.Serializable]
public class LOSUseEvent
{
    public float posX;
    public float posY;
    public float timestamp;

    public LOSUseEvent(Vector2 position, float time)
    {
        posX = position.x;
        posY = position.y;
        timestamp = time;
    }
}


[System.Serializable]
public class DeathReasonData
{
    public string reason;
    public float posX;
    public float posY;
    public float timestamp;

    public DeathReasonData(string reason, Vector2 position, float time)
    {
        this.reason = reason;
        posX = position.x;
        posY = position.y;
        timestamp = time;
    }
}

[System.Serializable]
public class EnemyKillData
{
    public string reason;
    public float posX;
    public float posY;
    public float timestamp;

    public EnemyKillData(string reason, Vector2 position, float time)
    {
        this.reason = reason;
        posX = position.x;
        posY = position.y;
        timestamp = time;
    }
}