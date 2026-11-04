using System.Collections.Generic;
using UnityEngine;

public enum WasteType
{ 
    GENERAL,
    ORGANIC,
    HAZARDOUS,
    ELECTRONIC
}

[CreateAssetMenu(menuName = "Waste/FacilityData")]
public class FacilityData : ScriptableObject
{
    public string displayName;
    [TextArea] public string description;
    public List<WasteType> acceptedWaste;
    public Facility prefab;
    public Vector2Int footprint = Vector2Int.one;
    public Vector2Int dockOffset;
}
