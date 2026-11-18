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
        Debug.Log("Tooltip subscribed");
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
        Debug.Log($"Show: {facility.name}");
        if (facility.Data == null) return;

        target = facility;
        FacilityData data = facility.Data;

        titleText.text = "Facility Type: " + data.displayName;
        descriptionText.text = "Description: " + data.description;
        acceptsText.text = "Accepts: " + FormatWaste(data.acceptedWaste);

        canvasGroup.alpha = 1f;
        Debug.Log($"Tooltip at {rectTransform.position}, size {rectTransform.rect.size}");
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

        Vector2 screenPoint = cam.WorldToScreenPoint(anchor);

        //picks a side: facility on the left half of the screen → panel to its right, and vice versa
        bool showOnRight = screenPoint.x < Screen.width * 0.5f;
        rectTransform.pivot = new Vector2(showOnRight ? 0f : 1f, 0.5f);

        Vector2 offset = new Vector2(showOnRight ? screenOffset.x : -screenOffset.x, screenOffset.y);
        Vector2 position = screenPoint + offset;

        // Keep the whole panel on screen
        Vector2 size = rectTransform.rect.size * rectTransform.lossyScale.x; // panel size in screen pixels
        Vector2 pivot = rectTransform.pivot;
        position.x = Mathf.Clamp(position.x, size.x * pivot.x, Screen.width - size.x * (1f - pivot.x));
        position.y = Mathf.Clamp(position.y, size.y * pivot.y, Screen.height - size.y * (1f - pivot.y));

        rectTransform.position = position;
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