using UnityEngine;

public class WasteProductionScript : MonoBehaviour
{
    public string streetBlock;
    public int currentTotalWaste;
    public int wasteProductionAmount;

    public int productionDayIntervals;

    private int dayCounter = 0;

    public void ProccessDailyProduction()
    {
        dayCounter++;

        if(dayCounter >= productionDayIntervals)
        {
            dayCounter = 0;
            currentTotalWaste += wasteProductionAmount;
        }
    }

    public int CollectTrash(int truckAvailableSpace)
    {
        int amountToCollect = Mathf.Min(currentTotalWaste, truckAvailableSpace);

        currentTotalWaste -= amountToCollect;

        return amountToCollect;
    }
}
