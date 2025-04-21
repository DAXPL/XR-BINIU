using UnityEngine;

public interface ISensor
{
    public object ReadData();
    public string GetDeviceName();
}
