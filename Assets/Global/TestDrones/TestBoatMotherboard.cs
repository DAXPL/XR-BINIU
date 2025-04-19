using UnityEngine;
using UnityEngine.InputSystem;

public class TestBoatMotherboard : MonoBehaviour
{
    [Header("Sterowanie")]
    [SerializeField] private InputActionReference moveAction;

    [Header("Silniki")]
    [SerializeField] private BrushlessMotor leftMotor;
    [SerializeField] private BrushlessMotor rightMotor;

    [Range(0f, 1f)] public float basePower = 0.5f;       // minimalna moc "do przodu"
    [Range(0f, 1f)] public float turnPowerFactor = 0.5f; // jak bardzo wp³ywa skrêt

    private void FixedUpdate()
    {
        if (moveAction == null || leftMotor == null || rightMotor == null)
            return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // forward tylko w górê (W lub pad do przodu)
        float forward = Mathf.Max(0f, input.y); // 0..1
        float turn = input.x;                   // -1..1

        float leftPWM = Mathf.Clamp01(forward + turn * turnPowerFactor);
        float rightPWM = Mathf.Clamp01(forward - turn * turnPowerFactor);

        leftPWM = Mathf.Lerp(0f, 1f, leftPWM);  // brak basePower
        rightPWM = Mathf.Lerp(0f, 1f, rightPWM);

        leftMotor.SetPWM(leftPWM);
        rightMotor.SetPWM(rightPWM);
    }
}