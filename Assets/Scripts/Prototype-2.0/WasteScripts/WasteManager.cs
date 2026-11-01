using UnityEngine;
using System.Collections.Generic;

public class WasteManager : MonoBehaviour
{
    public List<WasteData> inpectorStartingWaste = new List<WasteData>();
    public List<ActiveWasteInstance> wasteList = new List<ActiveWasteInstance>();

    public void Start()
    {
        foreach (WasteData item in inpectorStartingWaste)
        {
            wasteList.Add(new ActiveWasteInstance(item));
        }
    }

    private void Update()
    {
        for (int i = wasteList.Count - 1; i >= 0; i--)
        {
            if (wasteList[i].TimerCountdown(Time.deltaTime))
            {
                HandleExpiredWaste(i);
            }
        }
    }

    public void AddWasteObjectToList(WasteData wasteData)
    {
        wasteList.Add(new ActiveWasteInstance(wasteData));
    }

    private void HandleExpiredWaste(int index)
    {
        ActiveWasteInstance expiredWaste = wasteList[index];
    
        wasteList.RemoveAt(index);
    }
}
