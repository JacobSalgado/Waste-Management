using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Allows the truck to follow a list of world-space waypoints, then destroys itself on arrivals
/// </summary>
public class Truck : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 20f;

    [SerializeField] private float turnSpeed = 540f;

    // FOR FUTURE SCRIPTS (currency amount, pickup logic, spawner) can listen for this
    public event Action<Truck> OnArrived;

    private List<Vector3> waypoints;
    private int currentIndex;

    /**
     * @brief call this after the truck instantiates
     */
    public void SetPath(List<Vector3> path)
    {
        waypoints = path;
        transform.position = waypoints[0];          // snap  onto starting node
        currentIndex = 1;                           // already standing on waypoint 0

        Vector3 firstDirection;
        if (path.Count > 1)
            firstDirection = path[1] - path[0];
        else
            firstDirection = Vector3.up;

        transform.rotation = GridSpace.FacingRotation(firstDirection);
    }

    private void Update()
    {
        if (waypoints == null) return;

        // walked off the end of the list
        if (currentIndex >= waypoints.Count)
        {
            Arrive();
            return;
        }

        Vector3 target = waypoints[currentIndex];
        Vector3 toTarget = target - transform.position;

        // turn smoothly toward the next waypoint, staying upright on the grid
        if (toTarget.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRotation = GridSpace.FacingRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        }

        // MoveTowards never overshoots, and Time.deltaTime makes speed frame-rate independent.
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (transform.position == target)
        {
            currentIndex++;
        }
    }

    private void Arrive()
    {
        waypoints = null;
        OnArrived?.Invoke(this);
        Destroy(gameObject);
    }
}
