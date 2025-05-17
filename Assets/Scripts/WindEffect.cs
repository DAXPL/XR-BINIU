using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WindEffect : MonoBehaviour {
    [Header("Wind strength variables")]
    [SerializeField] private float xScale = 1f;
    [SerializeField] private float speed = 1f;

    [Tooltip("Wind blowing direction")]
    [SerializeField] private Vector3 directionVector;

    private readonly List<Vector3> vertices = new();
    private Rigidbody rb;
    private Vector3 forceVector;
    private float windScale;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        GetAllVertices();
    }

    private void GetAllVertices() {
        vertices.Clear();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        foreach (Vector3 vertex in meshFilter.mesh.vertices) {

            if (vertices.Contains(vertex))
                continue;

            vertices.Add(vertex);
        }

    }

    private void FixedUpdate() {
        // Calculate base wind force vector
        forceVector = directionVector.normalized * speed;

        // Loop through each vertex
        foreach (Vector3 vertex in vertices) {
            // Get vertex world position
            Vector3 worldVertex = transform.TransformPoint(vertex) - directionVector / 10;

            // Add offset for raycast
            directionVector.y += vertex.y;

            // Skip applying force if there is an obstacle in front of the vertex
            if (Physics.Raycast(worldVertex, -directionVector, 10))
                continue;

            windScale = 1;

            // Check if there is an obstacle behind the vertex
            if (Physics.Raycast(worldVertex, directionVector, out RaycastHit hit, 10))
                windScale = hit.distance/10;

            // Add randomized wind to base force
            forceVector *= Mathf.PerlinNoise(Time.fixedTime * xScale, 0.0f) * windScale;

            // Apply force at the vertex position
            rb.AddForceAtPosition(forceVector, vertex);
        }
    }

}
