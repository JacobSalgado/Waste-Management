using System;
using System.Net;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float arriveDistance = 0.5f;

    public event Action<int> OnRouteComplete;

    private Rigidbody rb;
    private Vector3 endPoint;
    private int wasteCollected;
    private bool isDriving;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void BeginRoute(Vector3 target)
    {
        endPoint = target;
        wasteCollected = 0;
        isDriving = true;
    }

    void FixedUpdate()
    {
        if (!isDriving) return;

        Vector3 target = endPoint;
        target.y = rb.position.y; // stays on plane

        Vector3 toTarget = target - rb.position;

        if (Vector3.Distance(rb.position, target) <= arriveDistance)
        {
            isDriving = false;
            OnRouteComplete?.Invoke(wasteCollected);
            return;
        }

        rb.MovePosition(Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime));

        // TODO: Put here when the truck rotates
    }

    public void TeleportTo(Transform point)
    {
        transform.SetPositionAndRotation(point.position, point.rotation);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDriving && other.CompareTag("Waste"))
        {
            wasteCollected++;
            Destroy(other.gameObject);
        }
    }
}
