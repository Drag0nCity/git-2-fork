
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EyeVisionZone : MonoBehaviour
{
    [Header("Vision Settings")]
    [SerializeField, Min(1f)] private float visionRadius = 7f;
    [SerializeField, Range(1f, 180f)] private float visionAngle = 60f;
    [SerializeField, Min(3)] private int segments = 40;

    [Header("Visual")]
    [SerializeField] private Material visionMaterial;

    private Mesh visionMesh;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (visionMaterial != null)
            meshRenderer.material = visionMaterial;
        else
            Debug.LogWarning("EyeVisionZone: Vision Material is not assigned!", this);

        CreateVisionMesh();
        SetVisible(false);
    }

    private void CreateVisionMesh()
    {
        visionMesh = new Mesh();
        visionMesh.name = "Eye Vision Mesh";

        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 3];

        vertices[0] = Vector3.zero;

        float halfAngle = visionAngle * 0.5f;

        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(
                -halfAngle,
                halfAngle,
                (float)i / segments
            );

            float radians = angle * Mathf.Deg2Rad;

            vertices[i + 1] = new Vector3(
                Mathf.Sin(radians) * visionRadius,
                0f,
                Mathf.Cos(radians) * visionRadius
            );
        }

        for (int i = 0; i < segments; i++)
        {
            int triangleIndex = i * 3;

            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = i + 2;
        }

        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = visionMesh;
    }

    public void SetVisible(bool visible)
    {
        if (meshRenderer != null)
            meshRenderer.enabled = visible;
    }

    public bool ContainsPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition);

        float distance = new Vector2(
            localPosition.x,
            localPosition.z
        ).magnitude;

        float angle = Mathf.Atan2(
            localPosition.x,
            localPosition.z
        ) * Mathf.Rad2Deg;

        return localPosition.z >= 0f &&
               distance <= visionRadius &&
               Mathf.Abs(angle) <= visionAngle * 0.5f;
    }

    private void OnDestroy()
    {
        if (visionMesh != null)
            Destroy(visionMesh);
    }
}
