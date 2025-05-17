using UnityEngine;

public class Car : MonoBehaviour {
    private Rigidbody rb;
    public float speed = 20;
    public float turnSpeed = 20;
    public Checkpoints checkpoints;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public Rigidbody GetRigidBody() => rb;

    public void Drive(float forward, float turn) {
        //rb.AddTorque(new Vector3(0, turn * turnSpeed, 0), ForceMode.Force);
        //rb.AddForce(speed * forward * transform.forward, ForceMode.Force);

        //// Calculate movement
        //Vector3 moveDirection = transform.forward * forward * speed * Time.deltaTime;
        //Quaternion turnRotation = Quaternion.Euler(0f, turn * turnSpeed * Time.deltaTime, 0f);

        //// Apply movement and rotation
        //rb.MovePosition(rb.position + moveDirection);
        //rb.MoveRotation(rb.rotation * turnRotation);


        // Apply forward/backward force
        rb.AddForce(transform.forward * forward * speed * Time.fixedDeltaTime, ForceMode.Force);
        rb.AddTorque(Vector3.up * turn * turnSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void FixedUpdate() {
        float forward = 0f;
        float turn = 0f;

        if (Input.GetKey("w"))
            forward = 1f;
        else if (Input.GetKey("s"))
            forward = -1f;

        if (Input.GetKey("a"))
            turn = -1f;
        else if (Input.GetKey("d"))
            turn = 1f;

        Drive(forward, turn);
    }


    public void Stop() {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
