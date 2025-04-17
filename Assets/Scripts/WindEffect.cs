using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour {
    [SerializeField] private float xScale;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 directionVector;
    private readonly List<Vector3> vertices = new();
    private Rigidbody rb;
    private Vector3 forceVector;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        GetAllVertices();
    }

    public void GetAllVertices() {
        vertices.Clear();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        foreach (Vector3 vertex in meshFilter.mesh.vertices) {

            if (vertices.Contains(vertex))
                continue;

            vertices.Add(vertex);
        }

    }

    private void FixedUpdate() {
        forceVector = directionVector.normalized * speed;
        foreach (Vector3 vertex in vertices) {
            Vector3 worldVertex = transform.TransformPoint(vertex) - directionVector / 10;
            directionVector.y += vertex.y;

            // Check if something is in front of vertex
            if (Physics.Raycast(worldVertex, -directionVector, 10))
                continue;

            forceVector *= Mathf.PerlinNoise(Time.fixedTime * xScale, 0.0f);
            rb.AddForceAtPosition(forceVector, vertex);
        }
    }
}
