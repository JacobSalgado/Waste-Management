using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set; }

    public event Action<int> OnMoneyChanged;
    public event Action<int> OnDayChanged;
    public event Action<bool> OnSimStateChanged;

    [SerializeField] private int currentLandfillAmount;
    [SerializeField] private int maxLandfillAmount;


    [SerializeField] private int playerMoney = 500000;

    [SerializeField] private int currentDay = 1;

    [Header("Facilities")]
    [SerializeField] private FacilityManager facilityManager;
    [SerializeField] private WasteProductionScript wasteProductionLocation;

    [Header("Simulation")]
    [SerializeField] private WasteSpawner wasteSpawner;
    [SerializeField] private TruckController truckPrefab;
    [SerializeField] private Transform truckSpawnPoint;
    [SerializeField] private Transform truckEndPoint;
    [SerializeField] private Transform transferStationPoint;
    [SerializeField] private float spawnPhaseDuration = 10f;
    [SerializeField] private float transferStationDelay = 2f;


    public int PlayerMoney => playerMoney;
    public int CurrentDay => currentDay;

    private bool isSimActive = false;
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
        currentLandfillAmount += amount;

        if(currentLandfillAmount >= maxLandfillAmount)
        {
            currentLandfillAmount = maxLandfillAmount;
            TriggerGameOver();
        }
    }
    public void AdvanceDay()
    {
        currentDay++;
        OnDayChanged?.Invoke(currentDay);

        if(facilityManager != null)
        {
            ProcessfacilityDayPhase();
        }
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

                int moneyEarned = facility.GetCurrentProductionAmount();
                AddMoney(moneyEarned);
            }
        }
    }

    public bool TrySpendMoney(int amount)
    {
        if(playerMoney >= amount)
        {
            playerMoney -= amount;
            OnMoneyChanged?.Invoke(playerMoney);
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        OnMoneyChanged?.Invoke(playerMoney);
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetSimActive(bool active)
    {
        if (isSimActive == active) return;

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
        Vector3 driveDir = truckEndPoint.position - truckSpawnPoint.position;
        driveDir.y = 0f;
        TruckController truck = Instantiate(truckPrefab, truckSpawnPoint.position, Quaternion.LookRotation(driveDir));

        bool routeDone = false;
        int collected = 0;
        truck.OnRouteComplete += amount => { collected = amount; routeDone = true; };
        truck.BeginRoute(truckEndPoint.position);

        yield return new WaitUntil(() => routeDone);

        // Third phase: teleport to transfer station, then remove truck
        truck.TeleportTo(transferStationPoint);
        yield return new WaitForSeconds(transferStationDelay);
        Destroy(truck.gameObject);

        // Fourth phase: collected waste goes to facilities, overflow goes to landfill
        int overflow = facilityManager != null ? facilityManager.DeliverWaste(collected) : collected;

        int uncollected = wasteSpawner.ClearRemainingWaste(); // waste that the truck missed also goes to landfill

        DumpIntoLandfill(overflow + uncollected);
        AdvanceDay();
    }
}
