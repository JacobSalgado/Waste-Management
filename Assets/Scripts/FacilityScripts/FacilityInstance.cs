using UnityEngine;

[System.Serializable]
public class FacilityInstance : MonoBehaviour
{
    public FacilityTemplate template;
    public int currentUpgrade = 1;

    public int currentDaysElapsed = 0;
    public int currentStorageAmount = 0;

    public FacilityInstance(FacilityTemplate sharedTemplate)
    {
        this.template = sharedTemplate;
    }

    public int GetCurrentProductionTime()
    {
        int calculatedProductionTime = template.defaultProductionTime - (template.decreasedProductionTimePerUpgrade * (currentUpgrade - 1));

        return Mathf.Max(calculatedProductionTime, template.minProductionTime);
    }
    
    public int GetCurrentProductionAmount()
    {
        float curentProductionAmount = template.defaultMoneyProduction + (template.defaultMoneyProduction * (template.moneyProductionMultiplier * (currentUpgrade - 1)));

        return Mathf.RoundToInt(curentProductionAmount);
    }

    public int GetMaxStorage()
    {
        float currentMaxStorage = template.defaultMaxStorageAmount + (template.defaultMaxStorageAmount * (template.storageLevelMultiplier * (currentUpgrade - 1)));

        return Mathf.RoundToInt(currentMaxStorage);
    }
    
    public void AddToStorage(int amount)
    {
        int maxStorage = GetMaxStorage();
        currentStorageAmount += amount;

        if(currentStorageAmount > maxStorage)
        {
            currentStorageAmount = maxStorage;
        }
    }

    public int GetRemainingStorageSpace()
    {
        return GetMaxStorage() - currentStorageAmount;
    }

    public bool CanAffordStorageCost()
    {
        return currentStorageAmount >= template.storageConsumptionPerCycle;
    }

    public void ConsumeStorage()
    {
        currentStorageAmount -= template.storageConsumptionPerCycle;
    }

    public int GetUpgradeCost()
    {
         float currentUpgradeCost = template.defaultUpgradeCost + (template.defaultUpgradeCost * (template.upgradeCostMultiplier * (currentUpgrade - 1)));

        return Mathf.RoundToInt(currentUpgradeCost);
    }

    public void Upgrade()
    {
        currentUpgrade++;
    }
}