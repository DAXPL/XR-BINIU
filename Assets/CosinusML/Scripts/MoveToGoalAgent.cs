using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections;

public class MoveToGoalAgent : Agent {
    [SerializeField] private Checkpoints checkpoints;
    [SerializeField] private Car car;
    private Vector3 lastPosition;

    private void Awake() {
        car = GetComponent<Car>();
    }

    private void Start() {
        checkpoints.OnCarCorrectCheckpoint += CorrectCheckpoint;
        checkpoints.OnCarWrongCheckpoint += WrongCheckpoint;
        StartCoroutine(CountDistance());
    }

    private void CorrectCheckpoint(object sender, Checkpoints.CarCheckpointEventArgs e) {
        if (e.carTransform == transform) {
            int index = checkpoints.GetNextCheckpointIndex(transform) + 1;

            if (checkpoints.GetNextCheckpointIndex(transform) == 0) {
                CheckVelocity();
                AddReward(100f);
                EndEpisode();
                return;
            }

            AddReward(index / 2f);
        }
    }

    private void CheckVelocity() {
        if (Mathf.Abs(car.GetRigidBody().linearVelocity.x) > 1 && Mathf.Abs(car.GetRigidBody().linearVelocity.z) > 5) {
            AddReward(2f);
        }
    }

    private void WrongCheckpoint(object sender, Checkpoints.CarCheckpointEventArgs e) {
        if (e.carTransform == transform) {
            AddReward(-1f);
            EndEpisode();
        }
    }

    private IEnumerator CountDistance() {
        while (true) {
            lastPosition = transform.position;
            yield return new WaitForSeconds(1);

            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            if (distanceMoved < 0.1f) {
                AddReward(-1.0f);
                EndEpisode();
            }
        }
    }

    public override void OnEpisodeBegin() {
        lastPosition = transform.position;
        transform.position = new Vector3(UnityEngine.Random.Range(31f, 27f), 0.8f, UnityEngine.Random.Range(-40.6f, -43.4f));        
        transform.forward = Vector3.forward;
        checkpoints.ResetCheckpoint(transform);
        car.Stop();
    }

    public override void CollectObservations(VectorSensor sensor) {
        Vector3 checkpointForward = checkpoints.GetNextCheckpoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float forwardAmount = 0;
        float turnAmount = 0;

        switch (actions.DiscreteActions[0]) {
            case 0:
            forwardAmount = 0;
            break;

            case 1:
            forwardAmount = 1;
            break;

            case 2:
            forwardAmount = -1;
            break;
        }

        switch (actions.DiscreteActions[1]) {
            case 0:
            turnAmount = 0;
            break;

            case 1:
            turnAmount = 1;
            break;

            case 2:
            turnAmount = -1;
            break;
        }

        car.Drive(forwardAmount, turnAmount);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-0.1f);
        }
    }
}
