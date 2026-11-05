using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Facility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<Facility> HoverStarted;
    public static event Action<Facility> HoverEnded;

    [SerializeField] private FacilityData data;
    [SerializeField] private Transform toolTipAnchor;

    public FacilityData Data
    {
        get { return data; }
    }

    public Transform ToolTipAnchor
    {
        get { return toolTipAnchor; }
    }

    public Vector2Int OriginCell { get; private set; }

    public void Init(FacilityData data, Vector2Int originCell)
    {
        this.data = data;
        OriginCell = originCell;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverStarted?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverEnded?.Invoke(this);
    }
}
