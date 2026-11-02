using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;

    [SerializeField]
    private InputAction leftMouseClick =
        new InputAction("LeftClick", InputActionType.Button, "<Mouse>/leftButton");

    [SerializeField] private InputAction mousePosition =
        new InputAction("MousePosition", InputActionType.Value, "<Pointer>/position");

    private void Awake()
    {
        pathfinding = new Pathfinding(10, 10);
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        pathfinding.GetGrid().GetXY(worldPosition, out int x, out int y);
        Debug.Log($"world: {worldPosition} cell: ({x}, {y})");

        List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
        Debug.Log($"path: {(path == null ? "null" : path.Count + " nodes")}");
        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(
                    new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f,
                    new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f,
                    Color.green, 5f);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        worldPosition.z = 0f;
        return worldPosition;
    }

    private void OnEnable()
    {
        leftMouseClick.Enable();
        mousePosition.Enable();
        leftMouseClick.performed += OnLeftClick;
    }

    private void OnDisable()
    {
        leftMouseClick.performed -= OnLeftClick;
        leftMouseClick.Disable();
        mousePosition.Disable();
    }
}
