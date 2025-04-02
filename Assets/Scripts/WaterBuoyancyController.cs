using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class WaterBuoyancyController : MonoBehaviour {
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float submersionDepth = 4;
    [SerializeField] private float submergedForceMultipier = 2;
    [SerializeField] private float waterDrag = 2;
    [SerializeField] private float waterAngularDrag = 2;
    [SerializeField] private WaterSurface water;

    private List<Vector3> vertices = new();
    private Vector3 totalDragForce;
    private Vector3 totalTorque;

    private void Start() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        foreach (Vector3 vertex in meshFilter.mesh.vertices) {
            if (vertex.y > 0)
                continue;

            if (vertices.Contains(vertex))
                continue;

            vertices.Add(vertex);
        }
    }

    private void FixedUpdate() {
        int submergedFloaters = 0;
        totalDragForce = Vector3.zero;
        totalTorque = Vector3.zero;

        foreach (Vector3 vertex in vertices) {
            Vector3 floaterPos = transform.TransformPoint(vertex);
            // Get water height at floater position
            WaterSearchParameters projectionParams = new() {
                startPositionWS = floaterPos
            };

            if (!water.ProjectPointOnWaterSurface(projectionParams, out WaterSearchResult projectionResult))
                continue;

            float waterHeight = projectionResult.projectedPositionWS.y;

            // If floater is underwater, apply buoyancy
            if (floaterPos.y < waterHeight) {
                submergedFloaters++;

                // Calculate buoyancy force for this floater
                float submergedAmount = Mathf.Clamp01((waterHeight - floaterPos.y) / submersionDepth);
                Vector3 buoyancyForce = Mathf.Abs(Physics.gravity.y) * submergedAmount * submergedForceMultipier * Vector3.up;

                // Add drag forces
                totalDragForce += Time.fixedDeltaTime * waterDrag * -rb.linearVelocity;
                totalTorque += Time.fixedDeltaTime * waterAngularDrag * -rb.angularVelocity;

                // Apply force at floater position
                rb.AddForceAtPosition(buoyancyForce, floaterPos, ForceMode.Acceleration);
            }
        }

        // Apply total drag once to prevent excessive dampening
        if (submergedFloaters > 0) {
            rb.AddForce(totalDragForce, ForceMode.VelocityChange);
            rb.AddTorque(totalTorque, ForceMode.VelocityChange);
        }
    }

}


