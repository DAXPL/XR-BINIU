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
    [SerializeField] private InputActionReference setAutopilot;

    [SerializeField] private int gear = 5;
    [SerializeField] private bool readFromInputsystem = true;
    [SerializeField] private Vector2 input = Vector2.zero;
    private bool changedInput = false;
    private bool autopilot = false;
    public void SetGear(Single s)
    {
        gear = Mathf.Clamp(Mathf.RoundToInt(s * 10), 0, 10);
        readFromInputsystem = false;
    }

    public void SetTurn(Single s)
    {
        input.x += Mathf.Clamp((s-0.5f)*2.0f, -1f, 1f);
        readFromInputsystem = false;
    }

    public void SetPower(Single s)
    {
        input.y = Mathf.Clamp((s - 0.5f) * 2.0f, -1f, 1f);
        readFromInputsystem = false;
    }
    
    public override void Initialize()
    {
        readFromInputsystem = true;
        if (gearUp != null)
        {
            gearUp.action.performed += ctx =>
            {
                readFromInputsystem = true;
                gear = Mathf.Clamp(gear + 1, 1, 10);
            };
            gearUp.action.Enable();
        }

        if (gearDown != null)
        {
            gearDown.action.performed += ctx =>
            {
                readFromInputsystem = true;
                gear = Mathf.Clamp(gear - 1, 1, 10);
            };
            gearDown.action.Enable();
        }

        if (moveAction != null)
            moveAction.action.Enable();

        if (setAutopilot != null)
        {
            setAutopilot.action.performed += ctx =>
            {
                changedInput = true;
                autopilot = !autopilot;
            };
            setAutopilot.action.Enable();
        }
    }

    public void ToggleAutopilot()
    {
        changedInput = true;
        autopilot = !autopilot;
    }

    public override void Think(Breadboard board, float deltaTime)
    {
        if (changedInput)
        {
            changedInput = false;
            if(autopilot) board.GetControll();
            else board.Release();

            board.ToggleMiscellaneous(0, autopilot);
        }
        if (readFromInputsystem)
        {
            if (moveAction != null)
            {
                input = moveAction.action.ReadValue<Vector2>();
            }
        }
        // Skalowanie si³y sterowania przez bieg
        input *= (gear / 10f);

        float forward = Mathf.Max(0f, input.y);

        float leftPWM = Mathf.Clamp01(forward + input.x);
        float rightPWM = Mathf.Clamp01(forward - input.x);

        board.SetMotorPWM(0, leftPWM);
        board.SetMotorPWM(1, rightPWM);
    }
}