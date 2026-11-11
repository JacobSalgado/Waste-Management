using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private InputAction rightMouseClick =
        new InputAction("RightClick", InputActionType.Button, "<Mouse>/rightButton");
    [SerializeField]
    private InputAction mousePosition =
        new InputAction("MousePosition", InputActionType.Value, "<Pointer>/position");

    [SerializeField] private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool TryGetGridPoint(Vector2 screenPos, out Vector3 point)
    {
        Plane gridPlane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (gridPlane.Raycast(ray, out float distance))
        {
            point = ray.GetPoint(distance);
            return true;
        }
        point = default;
        return false;
    }
}
