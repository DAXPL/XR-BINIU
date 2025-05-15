using UnityEngine;

public interface ISensor
{
    public object ReadData();
    public string GetDeviceName();
    public void SetData(object value);
}
