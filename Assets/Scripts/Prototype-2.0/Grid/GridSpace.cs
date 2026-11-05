using UnityEngine;

/// <summary>
/// this is a helper for 3D objects standing on the XY-plane grid
/// can be used for trucks, waste, facilities, or anything that should stand upright on a tile
/// </summary>

public class GridSpace : MonoBehaviour
{
    public static readonly Vector3 Up = Vector3.back; // the direction a model's top should point, so it is out of the grid toward the camera

    public static Quaternion FacingRotation(Vector3 direction)
    {
        direction.z = 0f;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector3.up; // default: face grid "north"
        return Quaternion.LookRotation(direction.normalized, Up);
    }

    public static Quaternion Upright
    {
        get
        {
            return FacingRotation(Vector3.up);
        }
    } 
}
