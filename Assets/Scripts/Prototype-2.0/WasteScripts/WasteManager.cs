using UnityEngine;
using System.Collections.Generic;

public class WasteManager : MonoBehaviour
{
    public static WasteManager Instance { get; private set; }
    [SerializeField] private List<WasteData> possibleWasteTypes = new List<WasteData>();
    public List<ActiveWasteInstance> wasteList = new List<ActiveWasteInstance>();

    [SerializeField] private float baseSpawnInterval = 60f;
    [SerializeField] private float intervalReduction = 10f;
    private float currentSpawnInterval;

    [SerializeField] private float spawnIntervalRemaining;

    public void Start()
    {
        
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

        if(GameManager.Instance.IsShiftActive)
        {
            spawnIntervalRemaining -= Time.deltaTime;

            if (spawnIntervalRemaining <= 0f)
            {
                SpawnWaste();
                CalculateSpawnInterval();
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

    public void CalculateSpawnInterval()
    {
        int currentShift = GameManager.Instance.currentShift;
        currentSpawnInterval = Mathf.Max(10f, baseSpawnInterval - ((currentShift - 1) * intervalReduction));
        spawnIntervalRemaining = currentSpawnInterval;
    }
    private void SpawnWaste()
    {
        WasteData randomWaste = possibleWasteTypes[Random.Range(0, possibleWasteTypes.Count)];
        AddWasteObjectToList(randomWaste);
    }
}
