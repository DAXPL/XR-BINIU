using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Scriptable Objects/TestBoatMotherboard")]
public class TestBoatMotherboard : DroneLogic
{
    [Header("Sterowanie")]
    [SerializeField] private InputActionReference moveAction;

    [Range(0f, 1f)] public float basePower = 0.5f;       // minimalna moc "do przodu"
    [Range(0f, 1f)] public float turnPowerFactor = 0.5f; // jak bardzo wp³ywa skrêt

    public override void Think(Breadboard board, float deltaTime)
    {
        Vector2 input = Vector2.zero;
        if (moveAction != null) input = moveAction.action.ReadValue<Vector2>();
        float forward = Mathf.Max(0f, input.y);
        float turn = input.x;

        float leftPWM = Mathf.Clamp01(forward + turn * turnPowerFactor);
        float rightPWM = Mathf.Clamp01(forward - turn * turnPowerFactor);

        leftPWM = Mathf.Lerp(0f, 1f, leftPWM);  // brak basePower
        rightPWM = Mathf.Lerp(0f, 1f, rightPWM);

        board.SetMotorPWM(0, leftPWM);
        board.SetMotorPWM(1, rightPWM);
    }
}