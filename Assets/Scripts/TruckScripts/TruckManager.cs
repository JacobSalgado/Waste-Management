using UnityEngine;
using System.Collections.Generic;

public class TruckManager : MonoBehaviour
{
    [Header("Parking")]
    [SerializeField] private Transform truckParkingParent;
    [SerializeField] private float truckSpacing = 4.5f;

    [SerializeField] private List<TruckInstance> ownedTrucks = new List<TruckInstance>();
    private List<TruckController> parkedTrucks = new List<TruckController>();

    public List<TruckInstance> OwnedTrucks => ownedTrucks;
    public int TruckCount => ownedTrucks.Count;

    public void PurchaseTruck(TruckTemplate shopTemplate)
    {
        int truckCost = GetCurrentTruckCost(shopTemplate);

        if (GameManager.Instance.TrySpendMoney(truckCost))
        {
            TruckInstance newTruck = new TruckInstance(shopTemplate);
            ownedTrucks.Add(newTruck);
            SpawnParkedTruck(newTruck);

            GameManager.Instance.NotifyStatsChanged();
        }
        else
        {
            Debug.Log($"Not enough money for {shopTemplate.truckName} (${truckCost: N0})");
        }
    }

    public bool TryUpgradeTruck(TruckInstance truck)
    {
        if (GameManager.Instance.TrySpendMoney(truck.GetUpgradeCost()))
        {
            truck.Upgrade();
            GameManager.Instance.NotifyStatsChanged();
            return true;
        }
        return false;
    }

    public int GetOwnedTruckTypeCount(TruckTemplate template)
    {
        int count = 0;
        foreach (TruckInstance truck in ownedTrucks)
        {
            if (truck.template.truckID == template.truckID)
                count++;
        }
        return count;
    }

    public int GetCurrentTruckCost(TruckTemplate template)
    {
        float cost = template.defaultTruckCost + (template.defaultTruckCost * (template.truckCostMultiplier * GetOwnedTruckTypeCount(template)));
        return Mathf.RoundToInt(cost);
    }

    public int GetTotalCapacity()
    {
        int total = 0;
        foreach (TruckInstance truck in ownedTrucks)
            total += truck.GetCapacity();
        return total;
    }

    private void SpawnParkedTruck(TruckInstance data)
    {
        Vector3 spawnPosition = new Vector3(parkedTrucks.Count * truckSpacing, 0f, 0f);
        Quaternion spawnRotation = Quaternion.identity;

        if (truckParkingParent != null)
        {
            spawnPosition += truckParkingParent.position;
            spawnRotation = truckParkingParent.rotation;
        }
        TruckController parked = Instantiate(data.template.truckPrefab, spawnPosition, spawnRotation, truckParkingParent);
        parked.Initialize(data);
        parkedTrucks.Add(parked);
    }

    public void SetParkedTrucksVisible(bool visible)
    {
        foreach (TruckController truck in parkedTrucks)
        {
            if (truck != null)
                truck.gameObject.SetActive(visible);
        }
    }
}
