using UnityEngine;

public class Car : MonoBehaviour {
    [SerializeField] private float speed = 20;
    [SerializeField] private float turnSpeed = 20;
    [SerializeField] private Checkpoints checkpoints;
    private Rigidbody rb;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public Rigidbody GetRigidBody() => rb;

    public void Drive(float forward, float turn) {
        rb.AddForce(forward * speed * Time.fixedDeltaTime * transform.forward, ForceMode.Force);
        rb.AddTorque(Time.fixedDeltaTime * turn * turnSpeed * Vector3.up, ForceMode.Force);
    }

    private void FixedUpdate() {
        float forward = 0f;
        float turn = 0f;

        if (Input.GetKey(KeyCode.W))
            forward = 1f;
        else if (Input.GetKey(KeyCode.S))
            forward = -1f;

        if (Input.GetKey(KeyCode.A))
            turn = -1f;
        else if (Input.GetKey(KeyCode.D))
            turn = 1f;

        Drive(forward, turn);
    }


    public void Stop() {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
