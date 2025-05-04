using UnityEngine;

public interface IPWMMotor
{
    void SetPWM(float value);
    float GetPWM();
}
