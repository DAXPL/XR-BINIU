using UnityEngine;

public class CheckpointSingle : MonoBehaviour {
    private Checkpoints checkpoints;
    [SerializeField] private bool isFinalCheckpoint;

    public bool GetIsFinalCheckpoint() => isFinalCheckpoint;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            checkpoints.AgentThroughCheckpoint(this, other.transform);
        }
    }

    public void SetCheckpoints(Checkpoints checkpoints) {
        this.checkpoints = checkpoints;
    }
}
