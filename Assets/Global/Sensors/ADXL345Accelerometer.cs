using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ADXL345Accelerometer : MonoBehaviour, ISensor
{
    [Header("Sensor settings")]
    [SerializeField] private float noiseLevel = 0.02f;

    private Vector3 lastVelocity;
    private Vector3 acceleration;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 currentVelocity = rb.linearVelocity;
        acceleration = (currentVelocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = currentVelocity;
    }

    public object ReadData()
    {
        // Dodaj szum i przekszta�� do lokalnego uk�adu wsp�rz�dnych
        Vector3 noisyAccel = acceleration + Random.insideUnitSphere * noiseLevel;
        return transform.InverseTransformDirection(noisyAccel);
    }

    public string GetDeviceName()
    {
        return "ADXL345";
    }

    public void SetData(object value)
    {
        Debug.LogWarning("ADXL345Accelerometer sensor does not support setting data.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 accel = (Vector3)ReadData();
        Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(accel));
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(accel), 0.05f);
    }
}
