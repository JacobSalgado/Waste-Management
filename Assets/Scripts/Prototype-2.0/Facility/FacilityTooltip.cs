using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Shows a small panel above a facility while the pointer hovers it.
/// Put this on the Tooltip panel (a child of a Screen Space - Overlay canvas).
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class FacilityTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI acceptsText;

    [Tooltip("Pixels to shift the panel from the facility's screen position. Raise Y to sit higher.")]
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 40f);

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Camera cam;
    private Facility target;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = (RectTransform)transform;
        cam = Camera.main;

        // The tooltip must never block the pointer, or it steals the hover from the
        // facility underneath and flickers on/off every frame.
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        Hide();
    }

    private void OnEnable()
    {
        Facility.HoverStarted += Show;
        Facility.HoverEnded += HandleHoverEnded;
    }

    private void OnDisable()
    {
        Facility.HoverStarted -= Show;
        Facility.HoverEnded -= HandleHoverEnded;
    }

    private void Show(Facility facility)
    {
        if (facility.Data == null) return;

        target = facility;
        FacilityData data = facility.Data;

        titleText.text = "Facility Type: " + data.displayName;
        descriptionText.text = "Description: " + data.description;
        acceptsText.text = "Accepts: " + FormatWaste(data.acceptedWaste);

        canvasGroup.alpha = 1f;
        UpdatePosition();
    }

    private void HandleHoverEnded(Facility facility)
    {
        // Moving straight from one facility to another can fire the new Enter before
        // the old Exit — only hide if it's the facility we're currently showing.
        if (facility == target) Hide();
    }

    private void Hide()
    {
        target = null;
        canvasGroup.alpha = 0f; // stays active so the event subscriptions keep working
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            // Covers a facility being destroyed while hovered.
            if (canvasGroup.alpha > 0f) Hide();
            return;
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 anchor = target.ToolTipAnchor != null
            ? target.ToolTipAnchor.position
            : target.transform.position;

        // World point on the grid's right edge, level with the facility
        Grid<PathNode> grid = Pathfinding.Instance.GetGrid();
        Vector3 rightEdge = grid.GetWorldPosition(grid.GetWidth(), 0);
        rightEdge.y = anchor.y;

        float edgeScreenX = cam.WorldToScreenPoint(rightEdge).x;
        float facilityScreenY = cam.WorldToScreenPoint(anchor).y;

        rectTransform.position = new Vector2(edgeScreenX, facilityScreenY) + screenOffset;
    }

    // HAZARDOUS → Hazardous, joined with commas
    private static string FormatWaste(List<WasteType> waste)
    {
        if (waste == null || waste.Count == 0) return "None";

        List<string> names = new List<string>(waste.Count);
        foreach (WasteType type in waste)
        {
            string raw = type.ToString();
            names.Add(char.ToUpper(raw[0]) + raw.Substring(1).ToLower());
        }
        return string.Join(", ", names);
    }
}