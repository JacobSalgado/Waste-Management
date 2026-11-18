using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float moveTime;

    [Header("Input")]
    [SerializeField] private InputAction scrollAction;
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction rightMouseClick;

    [SerializeField] private Camera cam;

    private Vector3 dragOrigin;

    public Vector3 newPosition;

    private bool isDragging;

    [SerializeField] private float closestZoom = -20f;   // nearest to the grid
    [SerializeField] private float farthestZoom = -150f; // farthest from the grid

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        newPosition = transform.position;
    }

    private void Update()
    {
        /*if (Mouse.current.rightButton.isPressed)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
        }*/

        // zoom in and out with scroll wheel
        //float scrollY = scrollAction.action.ReadValue<Vector2>().y;
        //transform.position += transform.forward * scrollY * zoomSpeed;

        HandleDrag();
        HandleKeys();
        HandleZoom();
        ClampTarget();
        ApplyMovement();
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        Vector3 worldPosition = GetMouseWorldPosition();
        transform.position -= new Vector3(worldPosition.x, 0, worldPosition.y) * moveSpeed * Time.deltaTime;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, -Camera.main.transform.position.z));
        worldPosition.z = 0f;
        return worldPosition;
    }

    private void HandleDrag()
    {
        if (rightMouseClick.WasPressedThisFrame())      // press: remember the grabbed point
        {
            dragOrigin = GetMouseWorldPosition();
            isDragging = true;
        }

        if (isDragging && rightMouseClick.IsPressed())  // held: one small step per frame
        {
            newPosition += dragOrigin - GetMouseWorldPosition();
        }

        if (rightMouseClick.WasReleasedThisFrame())     // release: stop
        {
            isDragging = false;
        }
    }

    private void HandleKeys()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        newPosition += new Vector3(input.x, input.y, 0f) * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scrollValue = scrollAction.ReadValue<float>();
        if (scrollValue != 0)
        {
            newPosition.z += Mathf.Sign(scrollValue) * zoomSpeed;
            newPosition.z = Mathf.Clamp(newPosition.z, farthestZoom, closestZoom);
        }
    }

    private void ClampTarget()
    {
        if (Pathfinding.Instance == null) return;
        Grid<PathNode> gridBounds = Pathfinding.Instance.GetGrid();
        Vector3 minCorner = gridBounds.GetWorldPosition(0, 0);
        Vector3 maxCorner = gridBounds.GetWorldPosition(gridBounds.GetWidth(), gridBounds.GetHeight());

        // how much the camera can see at the grid plane
        float distance = -newPosition.z;
        float halfHeight = distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * cam.aspect;

        newPosition.x = ClampAxis(newPosition.x, minCorner.x + halfWidth, maxCorner.x - halfWidth);
        newPosition.y = ClampAxis(newPosition.y, minCorner.y + halfHeight, maxCorner.y - halfHeight);
    }

    private static float ClampAxis(float value, float low, float high)
    {
        if (low > high) return (low + high) * 0.5f;
        return Mathf.Clamp(value, low, high);
    }

    private void ApplyMovement()
    {
        if (isDragging)
            transform.position = newPosition;
        else
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * moveTime);
    }

    private void OnEnable()
    {
        scrollAction.Enable();
        moveAction.Enable();
        rightMouseClick.Enable();
    }

    private void OnDisable()
    {
        scrollAction.Disable();
        moveAction.Disable();
        rightMouseClick.Disable();
    }

}
