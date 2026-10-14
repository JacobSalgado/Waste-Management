using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance{get; private set; }


    [SerializeField] private int currentLandfillAmount;
    [SerializeField] private int maxLandfillAmount;

    [SerializeField] private int playerMoney = 500000;

    [SerializeField] private int currentDay = 1;

    [SerializeField] private FacilityManager facilityManager;
    [SerializeField] private WasteProductionScript wasteProductionLocation;

    public int PlayerMoney => playerMoney;
    public int CurrentDay => currentDay;

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
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over");
        SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
