using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Rigidbody))]
public class WaterBuoyancyController : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private WaterSurface water;

    [Header("Buoyancy Settings")]
    [Tooltip("How much of the object will be submerged.")]
    [SerializeField] private float submersionDepth = 4;

    [Tooltip("How strong will the object bounce back up when submerged.")]
    [SerializeField] private float submergedForceMultipier = 2;

    [Tooltip("Slows down movement in water. Higher values make the object stop faster.")]
    [SerializeField] private float waterDrag = 2;

    [Tooltip("Slows down rotation in water. Higher values make the object rotate less.")]
    [SerializeField] private float waterAngularDrag = 2;

    private Rigidbody rb;
    private readonly List<Vector3> vertices = new();
    private Vector3 totalForce;
    private Vector3 totalTorque;
    private int submergedFloaters;

    private void Awake() {
        rb = GetComponent<Rigidbody>();

        if (water == null) {
            Debug.LogError("No water surface attached.");
        }
    }

    private void Start() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        foreach (Vector3 vertex in meshFilter.mesh.vertices) {
            // Ignore vertices above object's local level of water
            if (vertex.y > 0)
                continue;

            // Skip loop to avoid duplicate values in the list
            if (vertices.Contains(vertex))
                continue;

            vertices.Add(vertex);
        }
    }

    private void FixedUpdate() {
        // Reset values
        submergedFloaters = 0;
        totalForce = Vector3.zero;
        totalTorque = Vector3.zero;

        foreach (Vector3 vertex in vertices) {
            // Transform vertex from local to world space
            Vector3 vertexPos = transform.TransformPoint(vertex);

            // Get water height at vertex position
            WaterSearchParameters projectionParams = new() {
                startPositionWS = vertexPos
            };

            // Skip loop if vertex is not over a water surface
            if (!water.ProjectPointOnWaterSurface(projectionParams, out WaterSearchResult projectionResult))
                continue;

            // Get water height at vertex
            float waterHeight = projectionResult.projectedPositionWS.y;

            // If vertex is underwater apply buoyancy
            if (vertexPos.y < waterHeight) {
                submergedFloaters++;

                // Calculate buoyancy force for vertex
                float submergedAmount = Mathf.Clamp01((waterHeight - vertexPos.y) / submersionDepth);
                Vector3 buoyancyForce = 
                    Mathf.Abs(Physics.gravity.y) * submergedAmount * submergedForceMultipier * Vector3.up;

                // Add forces to total force vector at position of this vertex
                totalForce += Time.fixedDeltaTime * waterDrag * -rb.linearVelocity;
                totalTorque += Time.fixedDeltaTime * waterAngularDrag * -rb.angularVelocity;

                // Apply buoyancy force at the vertex position
                rb.AddForceAtPosition(buoyancyForce, vertexPos, ForceMode.Acceleration);
            }
        }

        // Apply total water resistance Y force at once if any vertex is submerged
        if (submergedFloaters > 0) {
            rb.AddForce(totalForce, ForceMode.VelocityChange);
            rb.AddTorque(totalTorque, ForceMode.VelocityChange);
        }
    }

}


