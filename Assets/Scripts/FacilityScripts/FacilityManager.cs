using UnityEngine;
using System.Collections.Generic;

public class FacilityManager : MonoBehaviour
{

    [SerializeField] private WasteProductionScript targetCollection;
    [SerializeField] private Transform landfillLocation;
    [SerializeField] private Transform spawnPointParent;

    [SerializeField] private List<FacilityInstance> activeDataInstance = new List<FacilityInstance>();
    [SerializeField] private List<FacilityWorldObject> activeVisualBuildings = new List<FacilityWorldObject>();

    public List<FacilityInstance> ActiveDataInstance => activeDataInstance;

    public void PurchaseFacility(FacilityTemplate shopTemplate)
    {
        int facilityCost = GetCurrentFacilityCost(shopTemplate);

        if(GameManager.Instance.TrySpendMoney(facilityCost))
        {
            FacilityInstance newInstance = new FacilityInstance(shopTemplate);
            activeDataInstance.Add(newInstance);

            Vector3 spawnPositon = new Vector3(activeVisualBuildings.Count * 3.5f, 0f, 0f);
            if(spawnPointParent != null)
            {
                spawnPositon += spawnPointParent.position;
            }

            GameObject spawnedBuildingObject = Instantiate(shopTemplate.facilityPrefab, spawnPositon, Quaternion.identity, spawnPointParent);

            FacilityWorldObject visualScript = spawnedBuildingObject.GetComponent<FacilityWorldObject>();

            if(visualScript != null)
            {
                visualScript.dataInstance = newInstance;
                activeVisualBuildings.Add(visualScript);
            }
        }
    }

    public int GetOwnedFacilityTypeCount(FacilityTemplate template)
    {
        int count = 0;
        foreach (FacilityInstance facility in activeDataInstance)
        {
            if(facility.template.facilityID == template.facilityID)
                count++;
        }
        return count;
    }

    public int GetCurrentFacilityCost(FacilityTemplate template)
    {
        float currentFacilityCost = template.defaultFacilityCost + (template.defaultFacilityCost * (template.facilityCostMultiplier * (GetOwnedFacilityTypeCount(template) - 1)));

        return Mathf.RoundToInt(currentFacilityCost);
    }

    public int DeliverWaste(int amount)
    {
        int remaining = amount;

        foreach (FacilityInstance facility in activeDataInstance)
        {
            if (remaining <= 0) break;

            int space = facility.GetRemainingStorageSpace();
            if (space <= 0) continue;

            int delivered = Mathf.Min(space, remaining);
            facility.AddToStorage(delivered);
            remaining -= delivered;
        }
        return remaining;
    }
}
