using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent {
    [SerializeField] private Transform goalTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private Checkpoints checkpoints;
    private Transform spawnPos;
    private Car car;

    private void Awake() {
        car = GetComponent<Car>();
    }

    private void Start() {
        spawnPos = transform;
        checkpoints.OnCarCorrectCheckpoint += CorrectCheckpoint;
        checkpoints.OnCarWrongCheckpoint += WrongCheckpoint;
    }

    private void CorrectCheckpoint() {
        AddReward(1f);
    }

    private void WrongCheckpoint() {

        AddReward(-1f);
    }

    public override void OnEpisodeBegin() {
        transform.position = spawnPos.position;
        transform.forward = spawnPos.forward;
        checkpoints.ResetCheckpoint();
        car.Stop();
    }

    public override void CollectObservations(VectorSensor sensor) {
        Vector3 checkpointForward = checkpoints.GetNextCheckpoint();
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

    public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }


    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.TryGetComponent(out Wall _)) {
            AddReward(-0.5f);
        }

    }


    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.TryGetComponent(out Wall _)) {
            AddReward(-0.1f);
        }
    }

}
