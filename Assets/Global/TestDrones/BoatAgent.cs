using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BoatAgent : Agent
{
    private Vector3 initialPos;
    private Vector3 initialRot;
    private Breadboard breadboard;
    private List<Transform> checkpoints = new List<Transform>();
    public int localCounter = 0;
    private void Start()
    {
        initialRot = transform.localEulerAngles;
        initialPos = transform.localPosition;
        breadboard = GetComponent<Breadboard>();
        if (breadboard != null && breadboard.GetControll())
        {
            Debug.Log("Breadboard is controllable");
        }
        else
        {
            Debug.Log("Breadboard is not controllable");
        }
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = initialPos;
        transform.localEulerAngles = initialRot;
        breadboard.SetMotorPWM(0, 0);
        breadboard.SetMotorPWM(1, 0);
        checkpoints.Clear();
        localCounter = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((float)breadboard.GetSensorData(0));
        sensor.AddObservation((float)breadboard.GetSensorData(1));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move1 = actions.ContinuousActions[0];
        float move2 = actions.ContinuousActions[1];

        breadboard.SetMotorPWM(0, move1);
        breadboard.SetMotorPWM(1, move2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            if (checkpoints.Contains(other.transform))
            {
                AddReward(-10f);
            }
            else
            {
                checkpoints.Add(other.transform);
                AddReward(15f);
                localCounter++;
            }
            
        }else if (other.gameObject.CompareTag("Wall"))
        {
            AddReward(150f);
            EndEpisode();
            return;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AddReward(-50f);
        EndEpisode();
        return;
    }

}
