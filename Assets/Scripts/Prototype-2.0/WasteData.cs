using UnityEngine;
public enum WasteType
{
    Organic,
    Recyclable,
    Hazardous,
    Other
}
[CreateAssetMenu(fileName = "New Waste", menuName = "Scriptable Objects/Waste Template")]
public class WasteData : ScriptableObject
{

    public string wasteID;
    public string wasteName;
    public Sprite wasteIcon;


}

