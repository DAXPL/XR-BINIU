using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCam : MonoBehaviour
{
    [Header("Input Action References")]
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public InputActionReference upDownAction;
    public InputActionReference scrollAction;

    [Header("Settings")]
    public float sensitivity = 1f;
    public float baseSpeed = 5f;
    public float scrollMultiplier = 2f;

    private float speedMultiplier = 1f;

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        upDownAction.action.Enable();
        scrollAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        upDownAction.action.Disable();
        scrollAction.action.Disable();
    }

    private void Update()
    {
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        Vector2 look = lookAction.action.ReadValue<Vector2>();
        float upDown = upDownAction.action.ReadValue<float>();
        Vector2 scrollVec = scrollAction.action.ReadValue<Vector2>();
        float scroll = scrollVec.y;
        // Rotation
        transform.Rotate(Vector3.up, look.x * sensitivity, Space.World);
        transform.Rotate(Vector3.right, -look.y * sensitivity, Space.Self);

        // Adjust speed
        speedMultiplier += scroll * scrollMultiplier * Time.deltaTime;
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.1f, 100f);

        // Movement
        Vector3 direction = new Vector3(move.x, upDown, move.y);
        transform.Translate(direction * baseSpeed * speedMultiplier * Time.deltaTime, Space.Self);
    }
}
