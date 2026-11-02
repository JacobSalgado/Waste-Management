using CodeMonkey;
using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Testing : MonoBehaviour
{
    private Pathfinding pathfinding;

    [SerializeField] private InputAction leftMouseClick;

    private void Start() 
    {
        Pathfinding pathfinding = new Pathfinding(10, 10);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            pathfinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            List<PathNode> path = pathfinding.FindPath(0, 0, x, y);
            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(
                        new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, 
                        new Vector3(path[i + 1].x, path[i + 1].y) * 10f + Vector3.one * 5f, Color.green, 5f);
                }
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
}
