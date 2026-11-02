using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;

    [Header("Grid")]
    [SerializeField] private UnityEngine.Grid tilemapGrid;
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;

    //[Header("Visuals")]
    //[SerializeField] private PathfindingVisual pathfindingVisual;

    [Header("Trucks")]
    [SerializeField] private Truck truckPrefab;
    [SerializeField] private Vector2Int spawnCell = new Vector2Int(0, 0);

    [Header("Input")]
    [SerializeField] private InputAction leftMouseClick =
        new InputAction("LeftClick", InputActionType.Button, "<Mouse>/leftButton");
    [SerializeField] private InputAction rightMouseClick =
        new InputAction("RightClick", InputActionType.Button, "<Mouse>/rightButton");
    [SerializeField] private InputAction mousePosition =
        new InputAction("MousePosition", InputActionType.Value, "<Pointer>/position");
    
    private void Awake()
    {
        if (tilemapGrid != null)
        {
            pathfinding = new Pathfinding(gridWidth, gridHeight,
                tilemapGrid.cellSize.x, tilemapGrid.transform.position);
        }
        else
            pathfinding = new Pathfinding(gridWidth, gridHeight);
    }

    private void Start()
    {
        //pathfindingVisual.SetGrid(pathfinding.GetGrid());
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        pathfinding.GetGrid().GetXY(worldPosition, out int x, out int y);
        Debug.Log($"world: {worldPosition} cell: ({x}, {y})");

        List<Vector3> path = pathfinding.FindWorldPath(spawnCell.x, spawnCell.y, x, y);
        Debug.Log($"path: {(path == null ? "null" : path.Count + " nodes")}");

        if (path == null)
        {
            Debug.Log($"No path from {spawnCell} to ({x}, {y})");
            return;
        }

        SpawnTruck(path);

        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.green, 5f);
            }
        }
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        pathfinding.GetGrid().GetXY(worldPosition, out int x, out int y);
        pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        worldPosition.z = 0f;
        return worldPosition;
    }

    /**
     * @brief instantiates the truck
     * @param path path for the truck to follow
     */
    private void SpawnTruck(List<Vector3> path)
    {
        Truck truck = Instantiate(truckPrefab, path[0], Quaternion.identity);
        truck.SetPath(path);
        truck.OnArrived += HandleTruckArrived;
    }

    /**
     * @brief calls OnArrived event from truck script
     */
    private void HandleTruckArrived(Truck truck)
    {
        truck.OnArrived -= HandleTruckArrived;
        Debug.Log("Truck arrived");
        // TODO: return trips? or when modifying score
    }

    private void OnEnable()
    {
        leftMouseClick.Enable();
        mousePosition.Enable();
        leftMouseClick.performed += OnLeftClick;
        rightMouseClick.performed += OnRightClick;
    }

    private void OnDisable()
    {
        leftMouseClick.performed -= OnLeftClick;
        rightMouseClick.performed -= OnRightClick;
        leftMouseClick.Disable();
        mousePosition.Disable();
    }
}
