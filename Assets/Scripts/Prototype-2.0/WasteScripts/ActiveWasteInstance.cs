using UnityEngine;

[System.Serializable]
public class ActiveWasteInstance
{
    public WasteData wasteData;
    public float timeRemaining;

    public ActiveWasteInstance(WasteData data)
    {
        wasteData = data;
        timeRemaining = data.wasteLifetime;
    }

    public bool TimerCountdown(float deltaTime)
    {
        timeRemaining -= deltaTime;
        return timeRemaining <= 0;
    }
}
