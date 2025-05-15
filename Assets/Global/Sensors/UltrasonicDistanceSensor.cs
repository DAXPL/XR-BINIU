using UnityEngine;

public class UltrasonicDistanceSensor : MonoBehaviour, ISensor
{
    [Header("Sensor settings")]
    [SerializeField] [Tooltip("Max range in m")] private float maxDistance = 4.0f;
    [SerializeField] [Tooltip("Min range in m")] private float minDistance = 0.02f;
    [SerializeField] [Tooltip("Random noise level")] private float noiseLevel = 0.01f; // opcjonalny szum
    [SerializeField] private float detectionAngle = 5f;
    [SerializeField] private LayerMask obstacleLayers;

    private float lastDistance;
    private bool lastHit;

    public object ReadData()
    {
        RaycastHit hit;

        if (Physics.SphereCast(transform.position, Mathf.Tan(detectionAngle * Mathf.Deg2Rad) * maxDistance, transform.forward, out hit, maxDistance, obstacleLayers))
        {
            float distance = Mathf.Clamp(hit.distance + Random.Range(-noiseLevel, noiseLevel), minDistance, maxDistance);
            lastDistance = distance;
            lastHit = true;
            return distance;
        }
        lastDistance = maxDistance;
        lastHit = false;
        return maxDistance;
    }

    public string GetDeviceName()
    {
        return "Ultrasonic_HY-SRF05";
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = transform.forward;
        Vector3 endPoint = transform.position + direction * (lastDistance > 0 ? lastDistance : maxDistance);

        Gizmos.color = lastHit ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, endPoint);
        Gizmos.DrawWireSphere(endPoint, 0.05f);
    }

    public void SetData(object value)
    {
        Debug.LogWarning("Ultrasonic sensor does not support setting data.");
    }
}
