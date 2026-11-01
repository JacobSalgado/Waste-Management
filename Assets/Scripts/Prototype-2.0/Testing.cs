using UnityEngine;
using CodeMonkey.Utils;

public class Testing : MonoBehaviour
{
    private Grid grid;
    private void Start() 
    {
        grid = new Grid(10, 6, 10f, new Vector3 (500, 0)); 
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse click
        {
            grid.SetValue(UtilsClass.GetMouseWorldPosition(), 56);
        }
        if (Input.GetMouseButtonDown(1)) // right mouse click
        {
            grid.GetValue(UtilsClass.GetMouseWorldPosition());
        }
    }
}
