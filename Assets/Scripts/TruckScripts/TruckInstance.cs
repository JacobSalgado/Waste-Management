using UnityEngine;

[System.Serializable]
public class TruckInstance
{
    public TruckTemplate template;
    public int currentUpgrade = 1;

    public TruckInstance(TruckTemplate sharedTemplate)
    {
        template = sharedTemplate;
    }

    public int GetCapacity()
    {
        float capacity = template.defaultCapacity + (template.defaultCapacity * (template.capacityMultiplier * (currentUpgrade - 1)));
        return Mathf.RoundToInt(capacity);
    }

    public float GetSpeed()
    {
        float speed = template.defaultSpeed + (template.speedPerUpgrade * (currentUpgrade - 1));
        return Mathf.Min(speed, template.maxSpeed);
    }

    public int GetUpgradeCost()
    {
        float cost = template.defaultUpgradeCost + (template.defaultUpgradeCost * (template.upgradeCostMultiplier * (currentUpgrade - 1)));
        return Mathf.RoundToInt(cost);
    }

    public void Upgrade()
    {
        currentUpgrade++;
    }
}
