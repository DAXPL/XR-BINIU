using System;
using UnityEngine;

public class VirtualBreadboard : Breadboard
{
    [SerializeField] private VirtualIO[] devices;

    private VirtualIO GetDevice(bool isSensor, int index)
    {
        int c = 0;
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isInput == isSensor)
            {
                if (c == index)
                {
                    return devices[i];
                }
                else
                {
                    c++;
                }
            }
        }
        return null;
    }
    
    public new void SetMotorPWM(int index, float value)
    {
        VirtualIO virtualIO = GetDevice(false, index);
        if(virtualIO != null) virtualIO.value = value;
    }

    public new object GetSensorData(int index)
    {
        VirtualIO virtualIO = GetDevice(true, index);
        if (virtualIO != null) return virtualIO.value;
        return null;
    }

    public new int GetSensorCount()
    {
        int count = 0;
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isInput)
            {
                count++;
            }
        }
        return count;
    }

    public new int GetMotorCount()
    {
        int count = 0;
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isInput == false)
            {
                count++;
            }
        }
        return count;
    }

    public new bool GetControll()
    {
        return false;
    }
    
    public void SetSensorData(int index, float value)
    {
        VirtualIO virtualIO = GetDevice(true, index);
        if (virtualIO != null)  virtualIO.value = value;
    }
    public new void Release()
    {
        
    }

    [System.Serializable]
    class VirtualIO
    {
        public bool isInput;
        public float value;
    }
}
