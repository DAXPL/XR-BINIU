using System;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour {
    public event EventHandler<CarCheckpointEventArgs> OnCarCorrectCheckpoint;
    public event EventHandler<CarCheckpointEventArgs> OnCarWrongCheckpoint;

    public class CarCheckpointEventArgs : EventArgs {
        public Transform carTransform;

        public CarCheckpointEventArgs(Transform carTransform) {
            this.carTransform = carTransform;
        }
    }

    [SerializeField] private List<GameObject> checkpoints = new();
    [SerializeField] private List<Transform> carTranformList = new();
    private readonly List<CheckpointSingle> checkpointSingleList = new();
    private readonly List<int> nextCheckpointSingleIndexList = new();

    private void Awake() {
        foreach (GameObject checkpoint in checkpoints) { 
            CheckpointSingle checkpointSingle = checkpoint.GetComponent<CheckpointSingle>();
            checkpointSingle.SetCheckpoints(this);
            checkpointSingleList.Add(checkpointSingle);
        }

        foreach (Transform carTransform in carTranformList) {
            nextCheckpointSingleIndexList.Add(0);
        }
    }

    public int GetCheckpointsCount() => checkpoints.Count;

    public void AgentThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform) {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTranformList.IndexOf(carTransform)];

        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex) {

            nextCheckpointSingleIndexList[carTranformList.IndexOf(carTransform)]
                = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;

            
            OnCarCorrectCheckpoint?.Invoke(this, new CarCheckpointEventArgs(carTransform));
        } else {
            OnCarWrongCheckpoint?.Invoke(this, new CarCheckpointEventArgs(carTransform));
        }
    }

    public void ResetCheckpoint(Transform carTransform) {
        nextCheckpointSingleIndexList[carTranformList.IndexOf(carTransform)] = 0;
    }

    public Transform GetNextCheckpoint(Transform carTransform) {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTranformList.IndexOf(carTransform)];
        return checkpoints[nextCheckpointSingleIndex].transform;
    }

    public int GetNextCheckpointIndex(Transform carTransform) {
        return nextCheckpointSingleIndexList[carTranformList.IndexOf(carTransform)];
    }

}
