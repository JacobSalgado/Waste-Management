using UnityEngine;

[CreateAssetMenu(fileName = "TruckTemplate", menuName = "Scriptable Objects/TruckTemplate")]
public class TruckTemplate : ScriptableObject
{
    [Header("Info")]
    public string truckID;
    public string truckName;
    public TruckController truckPrefab;

    [Header("Cost")]
    public int defaultTruckCost = 50000;
    public float truckCostMultiplier = 0.25f;

    [Header("Capacity")]
    public int defaultCapacity = 10;
    public float capacityMultiplier = 0.5f;

    [Header("Speed")]
    public float defaultSpeed = 8f;
    public float speedPerUpgrade = 1f;
    public float maxSpeed = 16f;

    [Header("Upgrades")]
    public int defaultUpgradeCost = 25000;
    public float upgradeCostMultiplier = 0.5f;
}
