using System.Collections.Generic;
using UnityEngine;

public class Breadboard : MonoBehaviour
{
    [SerializeField] private DroneLogic logic;

    [Header("Devices")]
    [SerializeField] private MonoBehaviour[] devices;
    private List<IPWMMotor> motors = new List<IPWMMotor>();
    private List<ISensor> sensors = new List<ISensor>();
    private bool externalLogic = false;
    private void Awake()
    {
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i] is IPWMMotor motor)
                motors.Add(motor);
            else if (devices[i] is ISensor sensor)
                sensors.Add(sensor);
            else
                Debug.LogWarning($"Element {i} does not implement IPWMMotor or ISensor");
        }
    }

    private void Update()
    {
        if (logic && !externalLogic) logic.Think(this, Time.deltaTime);
    }

    public void SetMotorPWM(int index, float value)
    {
        if (index >= 0 && index < motors.Count && motors[index] != null)
            motors[index].SetPWM(value);
    }

    public object GetSensorData(int index)
    {
        if (index >= 0 && index < sensors.Count && sensors[index] != null)
            return sensors[index].ReadData();
        return null;
    }

    public int GetSensorCount()
    {
        return sensors.Count;
    }
   
    public int GetMotorCount()
    {
        return motors.Count;
    }

    public bool GetControll()
    {
        if(externalLogic)
        {
            return false;
        }
        else
        {
            externalLogic = true;
            return true;
        }
    }

    public void Release()
    {
        externalLogic = false;
    }
}
