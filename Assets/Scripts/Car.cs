using UnityEngine;

public class Car : MonoBehaviour {
    private Rigidbody rb;
    public float speed = 20;
    public float turnSpeed = 20;
    public Checkpoints checkpoints;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void Drive(float forward, float turn) {
        rb.AddTorque(new Vector3(0, turn * turnSpeed, 0), ForceMode.Force);
        rb.AddForce(speed * forward * transform.forward, ForceMode.Force);
    }

    //private void Update() {
    //    if (Input.GetKey("a")) {
    //        rb.AddTorque(new Vector3(0, -turnSpeed, 0), ForceMode.Force);
    //    }

    //    if (Input.GetKey("d")) {
    //        rb.AddTorque(new Vector3(0, turnSpeed, 0), ForceMode.Force);
    //    }

    //    if (Input.GetKey("w")) {
    //        rb.AddForce(transform.forward * speed, ForceMode.Force);
    //    }

    //    if (Input.GetKey("s")) {
    //        rb.AddForce(-transform.forward * speed, ForceMode.Force);
    //    }
    //}

    public void Stop() {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
