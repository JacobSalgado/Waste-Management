using System.Collections.Generic;
using UnityEngine;

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
