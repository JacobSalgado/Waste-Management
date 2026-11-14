using UnityEngine;
using System.Collections.Generic;

public class WasteManager : MonoBehaviour
{
    [SerializeField] private List<WasteData> possibleWasteTypes = new List<WasteData>();
    public List<ActiveWasteInstance> wasteList = new List<ActiveWasteInstance>();

    private float baseSpawnInterval = 60f;
    private float intervalReduction = 10f;
    private float currentSpawnInterval;

    public void Start()
    {
        
    }

    private void Update()
    {
        for (int i = wasteList.Count - 1; i >= 0; i--)
        {
            if (wasteList[i].TimerCountdown(Time.deltaTime))
            {
                HandleExpiredWaste(i);
            }
        }
    }

    public void AddWasteObjectToList(WasteData wasteData)
    {
        wasteList.Add(new ActiveWasteInstance(wasteData));
    }

    private void HandleExpiredWaste(int index)
    {
        ActiveWasteInstance expiredWaste = wasteList[index];
    
        wasteList.RemoveAt(index);
    }

    private void CalculateSpawnInterval()
    {
        int currentShift = GameManager.Instance.currentShift;
        currentSpawnInterval = Mathf.Max(10f, baseSpawnInterval - ((currentShift - 1) * intervalReduction));
    }
    private void SpawnWaste()
    {
        WasteData randomWaste = possibleWasteTypes[Random.Range(0, possibleWasteTypes.Count)];
        AddWasteObjectToList(randomWaste);
    }
}
