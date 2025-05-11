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
    private List<CheckpointSingle> checkpointSingleList = new();
    private List<int> nextCheckpointSingleIndexList = new();

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


    public void AgentThroughCheckpoint(CheckpointSingle checkpointSingle, Transform carTransform) {
        int nextCheckpointSingleIndex = nextCheckpointSingleIndexList[carTranformList.IndexOf(carTransform)];

        if (checkpointSingleList.IndexOf(checkpointSingle) == nextCheckpointSingleIndex) {
            Debug.Log("correct");
            nextCheckpointSingleIndexList[carTranformList.IndexOf(carTransform)]
                = (nextCheckpointSingleIndex + 1) % checkpointSingleList.Count;
            OnCarCorrectCheckpoint?.Invoke(this, new CarCheckpointEventArgs(carTransform));
        } else {
            Debug.Log("wrong");
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






    ////[SerializeField] private List<GameObject> checkpoints = new();
    //[SerializeField] private GameObject nextCheckpoint;
    //public int currentCheckpointIndex;


    //public event Action OnCarCorrectCheckpoint;
    //public void CarCorrectCheckpoint() {
    //    OnCarCorrectCheckpoint?.Invoke();
    //}

    //public event Action OnCarWrongCheckpoint;
    //public void CarWrongCheckpoint() {
    //    OnCarWrongCheckpoint?.Invoke();
    //}

    //public Vector3 GetNextCheckpoint() {
    //    return nextCheckpoint.transform.position;
    //}

    //public GameObject GetNextCheckpointGameobject() {
    //    return nextCheckpoint;
    //}

    //public void SetNextCheckpoint() {
    //    currentCheckpointIndex++;
    //    if (currentCheckpointIndex >= checkpoints.Count) {
    //        currentCheckpointIndex = 0;
    //    }
    //    nextCheckpoint = checkpoints[currentCheckpointIndex];
    //    CarCorrectCheckpoint();
    //}

    //public void ResetCheckpoint() {
    //    currentCheckpointIndex = 0;
    //    nextCheckpoint = checkpoints[currentCheckpointIndex];
    //}
}
