using UnityEngine;

public class StepperMotor : MonoBehaviour, IPWMMotor
{
    public enum StepperMode { Rotation, Linear }

    [Header("Ustawienia silnika")]
    [SerializeField] private StepperMode mode = StepperMode.Rotation;
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 90f;
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 180f;

    [Header("Debug")]
    [Range(0f, 1f)]
    [SerializeField] private float controlSignal;

    private float currentPosition;

    private void Awake()
    {
        if (target == null && transform.childCount > 0)
            target = transform.GetChild(0);
    }

    private void Update()
    {
        float targetValue = Mathf.Lerp(minValue, maxValue, controlSignal);
        currentPosition = Mathf.MoveTowards(currentPosition, targetValue, speed * Time.deltaTime);

        if (target == null) return;

        if (mode == StepperMode.Rotation)
        {
            target.localRotation = Quaternion.AngleAxis(currentPosition, Vector3.forward);
        }
        else if (mode == StepperMode.Linear)
        {
            target.localPosition = Vector3.forward.normalized * currentPosition;
        }
    }

    public void SetPWM(float value)
    {
        controlSignal = Mathf.Clamp01(value);
    }

    public float GetPWM()
    {
        return controlSignal;
    }
}
