using UnityEngine;
using System.Collections.Generic;

public class TruckManager : MonoBehaviour
{
    [SerializeField] private List<TruckInstance> ownedTrucks = new List<TruckInstance>();

    public List<TruckInstance> OwnedTrucks => ownedTrucks;
    public int TruckCount => ownedTrucks.Count;

    public void PurchaseTruck(TruckTemplate shopTemplate)
    {
        int truckCost = GetCurrentTruckCost(shopTemplate);

        if (GameManager.Instance.TrySpendMoney(truckCost))
        {
            ownedTrucks.Add(new TruckInstance(shopTemplate));
            // TODO: Stats Change Update
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
            // TODO: states changed here from gamemanager
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
}
