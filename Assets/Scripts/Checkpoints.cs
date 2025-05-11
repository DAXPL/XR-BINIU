using System;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour {
    [SerializeField] private List<GameObject> checkpoints = new();
    [SerializeField] private GameObject nextCheckpoint;
    public int currentCheckpointIndex;


    public event Action OnCarCorrectCheckpoint;
    public void CarCorrectCheckpoint() {
        OnCarCorrectCheckpoint?.Invoke();
    }
    
    public event Action OnCarWrongCheckpoint;
    public void CarWrongCheckpoint() {
        OnCarWrongCheckpoint?.Invoke();
    }

    public Vector3 GetNextCheckpoint() {
        return nextCheckpoint.transform.position;
    } 
    
    public GameObject GetNextCheckpointGameobject() {
        return nextCheckpoint;
    }

    public void SetNextCheckpoint() {
        currentCheckpointIndex++;
        if (currentCheckpointIndex >= checkpoints.Count) {
            currentCheckpointIndex = 0;
        }
        nextCheckpoint = checkpoints[currentCheckpointIndex];
        CarCorrectCheckpoint();
    }
    
    public void ResetCheckpoint() {
        currentCheckpointIndex = 0;
        nextCheckpoint = checkpoints[currentCheckpointIndex];
    }
}
