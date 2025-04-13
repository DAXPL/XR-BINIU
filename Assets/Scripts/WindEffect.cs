using System.Collections.Generic;
using UnityEngine;

public class WindEffect : MonoBehaviour {
    private readonly List<Vector3> vertices = new();
    [SerializeField] private float xScale;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 directionVector;
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
            forceVector *= Mathf.PerlinNoise(Time.fixedTime * xScale, 0.0f);
            rb.AddForceAtPosition(forceVector, vertex);
        }

    }

    private void OnDrawGizmos() {
        Vector3 forceVectorPreview = directionVector.normalized * speed;
        foreach (Vector3 vertex2 in vertices) {
            Vector3 vertex = transform.TransformPoint(vertex2);
            forceVectorPreview *= Mathf.PerlinNoise(Time.fixedTime * xScale, 0.0f);
            Gizmos.DrawLine(vertex, vertex + forceVectorPreview);
            Gizmos.color = Color.red;
        }
    }
}
