using UnityEngine;

[CreateAssetMenu(fileName = "New Facility", menuName = "Scriptable Objects/Facility Template")]
public class FacilityTemplate : ScriptableObject
{

    public string facilityID;
    public string facilityName;
    public GameObject facilityPrefab;

    public int defaultFacilityCost;
    public float facilityCostMultiplier;

    //public int defaultUpkeepCost;
    //public float upkeepCostMultiplier;

    public int defaultMoneyProduction;
    public float moneyProductionMultiplier;

    public int defaultProductionTime;
    public int minProductionTime;
    public int decreasedProductionTimePerUpgrade;

    public int defaultUpgradeCost;
    public float upgradeCostMultiplier;

    public int storageConsumptionPerCycle;
    public int defaultMaxStorageAmount;
    public float storageLevelMultiplier;

}
