using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IMU : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float complementaryFilterAlpha = 0.98f; // �yroskop: 98%, Akcelerometr: 2%

    private Rigidbody rb;

    private Vector3 angles; // pitch (X), roll (Z)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // �yroskop � integracja pr�dko�ci k�towej
        Vector3 gyroRotation = rb.angularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime;
        angles.x += gyroRotation.x; // pitch
        angles.z += gyroRotation.z; // roll

        // Akcelerometr � szacowanie pitch/roll z grawitacji
        Vector3 accel = transform.InverseTransformDirection(rb.linearVelocity - Physics.gravity); // mniej wi�cej przyspieszenie
        accel.Normalize();

        float accelPitch = Mathf.Atan2(accel.z, accel.y) * Mathf.Rad2Deg;
        float accelRoll = Mathf.Atan2(accel.x, accel.y) * Mathf.Rad2Deg;

        // Komplementarny filtr
        angles.x = complementaryFilterAlpha * angles.x + (1 - complementaryFilterAlpha) * accelPitch;
        angles.z = complementaryFilterAlpha * angles.z + (1 - complementaryFilterAlpha) * accelRoll;
    }

    public object ReadData()
    {
        return new Vector2(angles.x, angles.z); // pitch i roll
    }

    public string GetDeviceName()
    {
        return "SimpleIMU";
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(angles.x, 0, angles.z) * Vector3.forward * 0.5f);
    }
}
