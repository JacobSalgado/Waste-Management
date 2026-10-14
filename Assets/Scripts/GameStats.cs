public struct GameStats
{
    public int money;
    public int day;

    public int landfillAmount;
    public int landfillMax;
    public int maxDays;
    public bool isWeekComplete;

    public int facilityCount;
    public int facilityStorageUsed;
    public int facilityStorageMax;

    public int cityWasteCount;

    public float LandfillPercent => landfillMax > 0 ? (float)landfillAmount / landfillMax : 0f;
    public float FacilityStoragePercent => facilityStorageMax > 0 ? (float)facilityStorageUsed / facilityStorageMax : 0f;
}