using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct FacilityPlacement
{
    public FacilityData data;
    public Vector2Int cell;
}

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

    [Header("Facility Info")]
    [SerializeField] private List<FacilityPlacement> facilityPlacements;
    [SerializeField] private Transform facilitiesParent; // empty object to hold all instantiated facilities

    [Header("Welcome UI")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject directionsPanel;

    private readonly List<Facility> facilities = new List<Facility>();
    private readonly Dictionary<Vector2Int, Facility> facilitiesByCell = new Dictionary<Vector2Int, Facility>();
    
    private void Start()
    {
        //pathfindingVisual.SetGrid(pathfinding.GetGrid());
    }

    public void BeginGame()
    {
        CreateTileMapGrid();
        PlaceFacilities();
    }

    private void CreateTileMapGrid()
    {
        if (tilemapGrid != null)
        {
            pathfinding = new Pathfinding(gridWidth, gridHeight,
                tilemapGrid.cellSize.x, tilemapGrid.transform.position);
        }
        else
            pathfinding = new Pathfinding(gridWidth, gridHeight);
    }

    private void PlaceFacilities()
    {
        foreach (FacilityPlacement placement in facilityPlacements)
        {
            PlaceFacility(placement.data, placement.cell);
        }
        Debug.Log("Facilities count: " + facilitiesByCell.Count);
    }

    private void PlaceFacility(FacilityData data, Vector2Int originCell)
    {
        if (!CanPlace(data, originCell)) return;

        // work out the world position
        Vector3 facilityLocation;
        facilityLocation = GetFootprintCenter(originCell, data.footprint);

        // instantiate
        Facility facility = Instantiate(data.prefab, facilityLocation, GridSpace.Upright, facilitiesParent);

        // init
        facility.Init(data, originCell);
        facilities.Add(facility);

        // loop the footprint
        for (int dx = 0; dx < data.footprint.x; dx++)
        {
            for (int dy = 0; dy < data.footprint.y; dy++)
            {
                // work out which cell this is
                Vector2Int cell = originCell + new Vector2Int(dx, dy);
                Debug.Log("PlaceFacility loop: " + cell);

                // get that cell's node and make it unwalkable
                pathfinding.GetNode(cell.x, cell.y).SetIsWalkable(false);

                // register the cell in facilitiesByCell
                facilitiesByCell[cell] = facility;
            }
        }
    }

    private bool CanPlace(FacilityData data, Vector2Int originCell)
    {
        // validate -- bail out early if something is wrong
        if (data == null)
        {
            Debug.LogWarning($"Facility placement at {originCell} has no FacilityData assigned");
            return false;
        }
        if (data.prefab == null)
        {
            Debug.LogWarning($"{data.name} has no prefab assigned");
            return false;
        }
        if (data.footprint.x < 1 || data.footprint.y < 1)
        {
            Debug.LogWarning($"{data.name} has an invalid footprint {data.footprint}");
            return false;
        }


        // Footprint: every cell must be on the grid, unoccupied.
        for (int dx = 0; dx < data.footprint.x; dx++)
        {
            for (int dy = 0; dy < data.footprint.y; dy++)
            {
                Vector2Int cell = originCell + new Vector2Int(dx, dy);

                if (!IsCellFree(cell, out string reason))
                {
                    Debug.LogWarning($"Can't place {data.name} at {originCell}: cell {cell} {reason}");
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsCellFree(Vector2Int cell, out string reason)
    {
        if (!pathfinding.GetGrid().IsValidGridPosition(cell))
        {
            reason = "is off grid";
            return false;
        }
        if (facilitiesByCell.ContainsKey(cell))
        {
            reason = "is taken by another facility";
            return false;
        }
        if (!pathfinding.GetNode(cell.x, cell.y).isWalkable)
        {
            reason = "is not walkable";
            return false;
        }

        reason = null;
        return true;
    }

    private Vector3 GetFootprintCenter(Vector2Int originCell, Vector2Int footprint)
    {
        Grid<PathNode> grid = pathfinding.GetGrid();
        float cellSize = grid.GetCellSize();
        Vector3 corner = grid.GetWorldPosition(originCell.x, originCell.y);

        return corner + new Vector3(footprint.x * cellSize * 0.5f, footprint.y * cellSize * 0.5f, 0f);
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!Pause.GameIsPaused)
        {
            Vector3 worldPosition = GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(worldPosition, out int x, out int y);
            Vector2Int clickedCell = new Vector2Int(x, y);
            bool targetIsFacility = facilitiesByCell.ContainsKey(clickedCell);
            Debug.Log($"world: {worldPosition} cell: ({x}, {y})");

            List<Vector3> path = pathfinding.FindWorldPath(spawnCell.x, spawnCell.y, x, y, targetIsFacility);
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
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        pathfinding.GetGrid().GetXY(worldPosition, out int x, out int y);

        Vector2Int clickedCell = new Vector2Int(x, y);
        if (facilitiesByCell.ContainsKey(clickedCell)) return;

        PathNode node = pathfinding.GetNode(x, y);
        if (node == null) return;

        node.SetIsWalkable(!node.isWalkable);
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
        rightMouseClick.Enable();
        mousePosition.Enable();
        leftMouseClick.performed += OnLeftClick;
        rightMouseClick.performed += OnRightClick;
    }

    private void OnDisable()
    {
        leftMouseClick.performed -= OnLeftClick;
        rightMouseClick.performed -= OnRightClick;
        leftMouseClick.Disable();
        rightMouseClick.Disable();
        mousePosition.Disable();
    }
}
