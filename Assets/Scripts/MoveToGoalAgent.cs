using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using System.Collections;

public class MoveToGoalAgent : Agent {
    [SerializeField] private Checkpoints checkpoints;
    private Transform spawnPos;
    private Vector3 spawnPosVector3;
    [SerializeField] private Car car;

    private void Awake() {
        car = GetComponent<Car>();
    }

    private void Start() {
        spawnPos = transform;
        spawnPosVector3 = transform.position;
        checkpoints.OnCarCorrectCheckpoint += CorrectCheckpoint;
        checkpoints.OnCarWrongCheckpoint += WrongCheckpoint;
        StartCoroutine(CountDistance());
    }

    private void CorrectCheckpoint(object sender, Checkpoints.CarCheckpointEventArgs e) {
        if (e.carTransform == transform) {
            AddReward(10f);
        }
    }

    private void WrongCheckpoint(object sender, Checkpoints.CarCheckpointEventArgs e) {
        if (e.carTransform == transform) {
            AddReward(-10f);
        }
    }

    Vector3 lastPosition;
    IEnumerator CountDistance() {
        while (true) {
            lastPosition = transform.position;
            yield return new WaitForSeconds(1);
            if (Vector3.Distance(transform.position, lastPosition) < 2) {
                AddReward(-100f);
            }
        }
    }


    public override void OnEpisodeBegin() {
        lastPosition = transform.position;
        transform.position = spawnPosVector3;
        transform.forward = spawnPos.forward;
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
        if (collision.gameObject.TryGetComponent(out Wall _)) {
            AddReward(-10f);
        }

    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.TryGetComponent(out Wall _)) {
            AddReward(5f);
        }
    }


    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.TryGetComponent(out Wall _)) {
            AddReward(-5f);
        }
    }
}
