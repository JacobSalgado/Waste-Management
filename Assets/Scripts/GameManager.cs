using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set; }
    public event Action<bool> OnSimStateChanged;
    public event Action<GameStats> OnStatsChanged;
    public event Action OnWeekComplete;

    [Header("Landfill")]
    [SerializeField] private int currentLandfillAmount;
    [SerializeField] private int maxLandfillAmount = 50;


    [Header("Economy")]
    [SerializeField] private int playerMoney = 500000;
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int maxDays = 7;

    private bool isWeekComplete = false;
    private bool isGameOver = false;

    public int MaxDays => maxDays;
    public bool IsWeekComplete => isWeekComplete;


    [Header("Facilities")]
    [SerializeField] private FacilityManager facilityManager;
    [SerializeField] private WasteProductionScript wasteProductionLocation;

    [Header("Simulation")]
    [SerializeField] private WasteSpawner wasteSpawner;
    [SerializeField] private TruckManager truckManager;
    [SerializeField] private float laneSpacing = 4f;
    [SerializeField] private Transform truckSpawnPoint;
    [SerializeField] private Transform truckEndPoint;
    [SerializeField] private Transform transferStationPoint;
    [SerializeField] private float spawnPhaseDuration = 10f;
    [SerializeField] private float transferStationDelay = 2f;

    private int cityWasteCount;
    private bool isSimActive = false;

    public int PlayerMoney => playerMoney;
    public int CurrentDay => currentDay;
    public bool IsSimActive => isSimActive;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DumpIntoLandfill(int amount)
    {
        currentLandfillAmount = Mathf.Min(currentLandfillAmount + amount, maxLandfillAmount);
        NotifyStatsChanged();

        if(currentLandfillAmount >= maxLandfillAmount)
        {
            TriggerGameOver();
        }
    }
    private void AdvanceDay()
    {
        currentDay++;

        if(facilityManager != null)
        {
            ProcessfacilityDayPhase();
        }
        if (currentDay >= maxDays)
        {
            isWeekComplete = true;
            NotifyStatsChanged();
            OnWeekComplete?.Invoke();
            return;
        }

        currentDay++;
        NotifyStatsChanged();

    }

    private void ProcessfacilityDayPhase()
    {
        foreach(FacilityInstance facility in facilityManager.ActiveDataInstance)
        {
            if(facility.template.defaultMoneyProduction <= 0f)
            {
                continue;
            }

            if(!facility.CanAffordStorageCost())
            {
                continue;
            }

            facility.currentDaysElapsed++;

            if(facility.currentDaysElapsed >= facility.GetCurrentProductionTime())
            {
                facility.currentDaysElapsed = 0;
                facility.ConsumeStorage();

                //int moneyEarned = facility.GetCurrentProductionAmount();
                AddMoney(facility.GetCurrentProductionAmount());
            }
        }
    }

    public bool TrySpendMoney(int amount)
    {
        if(playerMoney >= amount)
        {
            playerMoney -= amount;
            NotifyStatsChanged();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        NotifyStatsChanged();
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetSimActive(bool active)
    {
        if (active && (isWeekComplete || isGameOver)) return;

        isSimActive = active;
        OnSimStateChanged?.Invoke(isSimActive);

        if (isSimActive)
        {
            StartCoroutine(RunSimulation());
        }
    }

    public void ToggleSim()
    {
        SetSimActive(!isSimActive);
    }

    private IEnumerator RunSimulation()
    {
        // First phase: waste spawns in
        wasteSpawner.StartSpawning();
        yield return new WaitForSeconds(spawnPhaseDuration);
        wasteSpawner.StopSpawning();

        // Second phase: truck collects waste
        List<TruckInstance> owned = truckManager.OwnedTrucks;
        if (owned.Count == 0) Debug.Log("No Trucks owned: all city waste goes to landfill");

        Vector3 driveDir = truckEndPoint.position - truckSpawnPoint.position;
        driveDir.y = 0f;
        Vector3 side = Vector3.Cross(Vector3.up, driveDir.normalized);
        Quaternion facing = Quaternion.LookRotation(driveDir);

        List<TruckController> activeTrucks = new List<TruckController>();
        int trucksFinished = 0;
        int collected = 0;

        for (int i = 0; i < owned.Count; i++)
        {
            float laneOffset = (i - (owned.Count - 1) / 2f) * laneSpacing;
            Vector3 start = truckSpawnPoint.position + side * laneOffset;
            Vector3 end = truckEndPoint.position + side * laneOffset;

            TruckController truck = Instantiate(owned[i].template.truckPrefab, start, facing);
            truck.Initialize(owned[i]);
            truck.OnRouteComplete += amount => { collected += amount; trucksFinished++; };
            truck.BeginRoute(end);
            activeTrucks.Add(truck);
        }

        yield return new WaitUntil(() => trucksFinished >= activeTrucks.Count);

        // Third phase: teleport to transfer station, then remove truck
        foreach (TruckController truck in activeTrucks)
            truck.TeleportTo(transferStationPoint);

        yield return new WaitForSeconds(transferStationDelay);

        foreach (TruckController truck in activeTrucks)
            Destroy(truck.gameObject);

        // Fourth phase: collected waste goes to facilities, overflow goes to landfill
        int overflow = facilityManager != null ? facilityManager.DeliverWaste(collected) : collected;

        int uncollected = wasteSpawner.ClearRemainingWaste(); // waste that the truck missed also goes to landfill
        cityWasteCount = 0;

        DumpIntoLandfill(overflow + uncollected);
        if (isGameOver) yield break;

        AdvanceDay();
        SetSimActive(false);
    }

    // ---------- Stats Info ----------
    public GameStats GetStats()
    { 
        return new GameStats
        { 
            money = playerMoney,
            day = currentDay,
            landfillAmount = currentLandfillAmount,
            landfillMax = maxLandfillAmount,
            maxDays = maxDays,
            isWeekComplete = isWeekComplete,
            facilityCount = facilityManager != null ? facilityManager.FacilityCount: 0,
            facilityStorageUsed = facilityManager != null ? facilityManager.GetTotalStorageUsed() : 0,
            facilityStorageMax = facilityManager != null ? facilityManager.GetTotalMaxStorage() : 0,
            cityWasteCount = cityWasteCount,
        };
    }
    public void NotifyStatsChanged()
    {
        OnStatsChanged?.Invoke(GetStats());
    }

    public void RegisterWasteSpawned()
    {
        cityWasteCount++;
        NotifyStatsChanged();
    }

    public void RegisterWasteCollected()
    {
        cityWasteCount = Mathf.Max(0, cityWasteCount - 1);
        NotifyStatsChanged();
    }
}
