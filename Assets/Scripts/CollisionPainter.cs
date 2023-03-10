using UnityEngine;

public class CollisionPainter : MonoBehaviour
{
    public float paintRange = 1.0f;
    public Color paintColor = Color.white;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;
    private bool[] paintedVertices;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
        vertices = mesh.vertices;
        paintedVertices = new bool[vertices.Length];
    }

    void OnCollisionEnter(Collision collision)
    {
        // Get the point of contact between the collision and the object
        Vector3 contactPoint = collision.contacts[0].point;

        // Find the closest vertex to the contact point in world space
        Vector3 closestVertex = transform.TransformPoint(vertices[0]);
        float closestDistance = Vector3.Distance(closestVertex, contactPoint);
        for (int i = 1; i < vertices.Length; i++)
        {
            Vector3 vertex = transform.TransformPoint(vertices[i]);
            float distance = Vector3.Distance(vertex, contactPoint);
            if (distance < closestDistance)
            {
                closestVertex = vertex;
                closestDistance = distance;
            }
        }

        // Paint the closest vertex and any other vertices within the range
        float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        float scaledPaintRange = paintRange / scale;
        Quaternion inverseRotation = Quaternion.Inverse(transform.rotation);
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = transform.TransformPoint(vertices[i]);
            Vector3 vertexLocalSpace = inverseRotation * (vertex - transform.position);
            float distance = Vector3.Distance(vertex, closestVertex) * scale;
            if (distance < scaledPaintRange)
            {
                paintedVertices[i] = true;
            }
        }

        // Update the mesh colors
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            if (paintedVertices[i])
            {
                colors[i] = paintColor;
            }
            else
            {
                colors[i] = Color.white;
            }
        }
        mesh.colors = colors;
    }
}
