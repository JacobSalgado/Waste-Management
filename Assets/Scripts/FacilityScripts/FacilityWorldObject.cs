using UnityEngine;

public class FacilityWorldObject : MonoBehaviour
{
    public FacilityInstance dataInstance;

    public int GetSpaceAvailabe()
    {
        if(dataInstance == null)
            return 0;
        
        return dataInstance.GetRemainingStorageSpace();
    }

    public void ReveiveWaste(int amount)
    {
        if(dataInstance != null)
        {
            dataInstance.AddToStorage(amount);
        }
    }
}
