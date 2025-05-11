using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AirDromeAgent : Agent
{
    private Vector3 initialPos;
    private Vector3 initialRot;
    private Breadboard breadboard;
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
        breadboard.SetMotorPWM(2, 0);
        breadboard.SetMotorPWM(3, 0);
        localCounter = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((float)breadboard.GetSensorData(0));
        sensor.AddObservation((float)breadboard.GetSensorData(1));
        sensor.AddObservation((float)breadboard.GetSensorData(2));
        sensor.AddObservation((float)breadboard.GetSensorData(3));
        sensor.AddObservation((float)breadboard.GetSensorData(4));
        sensor.AddObservation((float)breadboard.GetSensorData(5));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float move1 = actions.ContinuousActions[0];
        float move2 = actions.ContinuousActions[1];
        float move3 = actions.ContinuousActions[2];
        float move4 = actions.ContinuousActions[3];
        breadboard.SetMotorPWM(0, move1);
        breadboard.SetMotorPWM(1, move2);
        breadboard.SetMotorPWM(2, move3);
        breadboard.SetMotorPWM(3, move4);
    }


    private void OnCollisionEnter(Collision collision)
    {
        AddReward(-50f);
        EndEpisode();
        return;
    }
}
