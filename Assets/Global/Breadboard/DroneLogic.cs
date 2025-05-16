using UnityEngine;

public abstract class DroneLogic : ScriptableObject
{
    public abstract void Initialize();
    public abstract void Think(Breadboard board, float deltaTime);
}
