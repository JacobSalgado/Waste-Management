using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float zoomSpeed = 5f;
    private Vector3 lastMousePosition;

    [Header("Input")]
    [SerializeField]
    private InputAction rightMouseClick =
        new InputAction("RightClick", InputActionType.Button, "<Mouse>/rightButton");
    [SerializeField]
    private InputAction mousePosition =
        new InputAction("MousePosition", InputActionType.Value, "<Pointer>/position");
    [SerializeField] private InputActionReference scrollAction;

    private void Update()
    {
        // zoom in and out with scroll wheel
        float scrollY = scrollAction.action.ReadValue<Vector2>().y;
        transform.position += transform.forward * scrollY * zoomSpeed;
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

    private void OnEnable()
    {
        rightMouseClick.Enable();
        mousePosition.Enable();
        rightMouseClick.performed += OnRightClick;
    }

    private void OnDisable()
    {
        rightMouseClick.performed -= OnRightClick;
        rightMouseClick.Disable();
        mousePosition.Disable();
    }
}
