using UnityEngine;

public class FacilityManager : MonoBehaviour
{
    [SerializeField] private List<FacilityInstance> activeDataInstance = new List<FacilityInstance>();

    public List<FacilityInstance> ActiveDataInstance => activeDataInstance;

    public void PurchaseFacility(FacilityTemplate shopTemplate)
    {
        float facilityCost = GetCurrentFacilityCost(shopTemplate);

        if(GameManager.Instance.TrySpendMoney(cost))
        {
            FacilityInstance newInstance = new FacilityInstance(shopTemplate);
            activeDataInstance.Add(newInstance);
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
}
