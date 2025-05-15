using System.Collections.Generic;
using UnityEngine;

public class Breadboard : MonoBehaviour
{
    [SerializeField] private DroneLogic logic;

    [Header("Devices")]
    [SerializeField] private MonoBehaviour[] devices;
    [SerializeField] private VirtualSensor[] virtualSensors;
    [SerializeField] private VirtualMotor[] virtualMotors;
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
        // Obs³uga wirtualnych sensorów
        for (int i = 0; i < virtualSensors.Length; i++)
        {
            if (virtualSensors[i] is ISensor sensor)
                sensors.Add(sensor);
            else
                Debug.LogWarning($"VirtualSensor {i} does not implement ISensor");
        }

        // Obs³uga wirtualnych motorów
        for (int i = 0; i < virtualMotors.Length; i++)
        {
            if (virtualMotors[i] is IPWMMotor motor)
                motors.Add(motor);
            else
                Debug.LogWarning($"VirtualMotor {i} does not implement IPWMMotor");
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

    public float GetMotorPWM(int index)
    {
        if (index >= 0 && index < motors.Count && motors[index] != null)
            return motors[index].GetPWM();
        return 0f;
    }

    public object GetSensorData(int index)
    {
        if (index >= 0 && index < sensors.Count && sensors[index] != null)
            return sensors[index].ReadData();
        return null;
    }
    
    public void SetSensorData(int index, object value)
    {
        Debug.Log($"SetSensorData: {index} {value}");
        if (index >= 0 && index < sensors.Count && sensors[index] != null)
            sensors[index].SetData(value);
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
[System.Serializable]
public class VirtualSensor : ISensor
{
    [SerializeField] private float sensorValue;
    public string GetDeviceName()
    {
        return "VirtualSensor";
    }

    public object ReadData()
    {
        return sensorValue;
    }

    public void SetData(object value)
    {
        sensorValue = (float)value;
    }
}
[System.Serializable]
public class VirtualMotor : IPWMMotor
{
    [SerializeField] private float pwmValue;
    public void SetPWM(float value)
    {
        pwmValue = value;
    }
    public float GetPWM()
    {
        return pwmValue;
    }
}