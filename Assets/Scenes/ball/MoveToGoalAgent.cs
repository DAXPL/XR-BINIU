using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.InputSystem;
// https://www.youtube.com/watch?v=zPFU30tbyKs&list=PLzDRvYVwl53vehwiN_odYJkPBzcqFw110
// https://www.youtube.com/watch?v=RANRz9oyzko
public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private InputActionReference moveAction;

    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private Renderer floorRenderer;
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-6f,6f),0, Random.Range(-4f, 4f));
        targetTransform.localPosition = new Vector3(Random.Range(-6f, 6f), 0, Random.Range(-4f, 4f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5f;
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            SetReward(1f);
            floorRenderer.material = winMaterial;
            EndEpisode();
        }
        else if (other.CompareTag("Wall"))
        {
            SetReward(-1f);
            floorRenderer.material = loseMaterial;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = moveAction.action.ReadValue<Vector2>().x;
        continuousActions[1] = moveAction.action.ReadValue<Vector2>().y;
    }
}
