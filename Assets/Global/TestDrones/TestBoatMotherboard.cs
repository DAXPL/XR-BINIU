using System;
using Unity.MLAgents.Integrations.Match3;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Scriptable Objects/TestBoatMotherboard")]
public class TestBoatMotherboard : DroneLogic
{
    [Header("Sterowanie")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference gearUp;
    [SerializeField] private InputActionReference gearDown;

    private int gear = 5;
    private bool initialized = false;

    private void Initialize()
    {
        initialized = true;

        if (gearUp != null)
        {
            gearUp.action.performed += ctx =>
            {
                gear = Mathf.Clamp(gear + 1, 1, 10);
                Debug.Log("Zwiêkszono bieg: " + gear);
            };
            gearUp.action.Enable();
        }

        if (gearDown != null)
        {
            gearDown.action.performed += ctx =>
            {
                gear = Mathf.Clamp(gear - 1, 1, 10);
                Debug.Log("Zmniejszono bieg: " + gear);
            };
            gearDown.action.Enable();
        }

        if (moveAction != null)
            moveAction.action.Enable();
    }

    public override void Think(Breadboard board, float deltaTime)
    {
        if (!initialized) Initialize();

        Vector2 input = Vector2.zero;
        if (moveAction != null)
            input = moveAction.action.ReadValue<Vector2>();

        // Skalowanie si³y sterowania przez bieg
        input *= gear / 10f;

        float forward = Mathf.Max(0f, input.y);
        float turn = input.x;

        float leftPWM = Mathf.Clamp01(forward + turn);
        float rightPWM = Mathf.Clamp01(forward - turn);

        board.SetMotorPWM(0, leftPWM);
        board.SetMotorPWM(1, rightPWM);
    }
}