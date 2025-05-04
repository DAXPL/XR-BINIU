using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BrushlessMotor : MonoBehaviour, IPWMMotor
{
    [Header("Silnik")]
    [SerializeField] private float kv = 1000f;              // rpm/V
    [SerializeField] private float voltage = 12f;           // napiêcie zasilania
    [SerializeField] private float maxThrust = 10f;         // maksymalny ci¹g [N]
    [SerializeField] private float thrustCoefficient = 1f;  // si³a wzglêdna (skalowanie)

    [Header("Sterowanie")]
    [Range(0f, 1f)]
    [SerializeField] private float pwmInput = 0f;

    private Rigidbody rb;
    private float currentRPM;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Oblicz RPM i ci¹g
        float maxRPM = kv * voltage;
        currentRPM = pwmInput * maxRPM;

        float normalizedPower = Mathf.Pow(currentRPM / maxRPM, 2);
        float thrust = normalizedPower * maxThrust * thrustCoefficient;

        Vector3 force = transform.forward * thrust * Time.fixedDeltaTime;
        rb.AddForce(force);
    }

    public void SetPWM(float value)
    {
        pwmInput = Mathf.Clamp01(value);
    }

    public float GetThrust()
    {
        return Mathf.Pow(pwmInput * kv * voltage / (kv * voltage), 2) * maxThrust;
    }

    public float GetPWM()
    {
        return pwmInput;
    }
}
