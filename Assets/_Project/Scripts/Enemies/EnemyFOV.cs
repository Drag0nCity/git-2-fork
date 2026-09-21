using UnityEngine;

public sealed class EnemyFOV : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float detectionAngle = 90f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Game View Debug")]
    [SerializeField] private bool showDebugInGame = true;
    [SerializeField] private Color fovColor = Color.yellow;
    [SerializeField] private Color targetColor = Color.red;
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private int circleSegments = 48;

    private readonly Collider[] targetBuffer = new Collider[16];

    private LineRenderer fovCircle;
    private LineRenderer leftLine;
    private LineRenderer rightLine;
    private LineRenderer targetLine;

    public Transform CurrentTarget { get; private set; }

    private void Awake()
    {
        if (showDebugInGame)
        {
            CreateDebugLines();
        }
    }

    public bool TryFindTarget()
    {
        int targetCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            detectionRadius,
            targetBuffer,
            targetMask
        );

        for (int i = 0; i < targetCount; i++)
        {
            Collider targetCollider = targetBuffer[i];

            if (targetCollider == null)
            {
                continue;
            }

            Vector3 directionToTarget =
                targetCollider.bounds.center - transform.position;

            float distanceToTarget =
                directionToTarget.magnitude;

            if (distanceToTarget <= 0f)
            {
                continue;
            }

            float angle = Vector3.Angle(
                transform.forward,
                directionToTarget
            );

            if (angle > detectionAngle * 0.5f)
            {
                continue;
            }

            Vector3 origin =
                transform.position + Vector3.up;

            Vector3 targetPosition =
                targetCollider.bounds.center;

            Vector3 direction =
                targetPosition - origin;

            if (Physics.Raycast(
                    origin,
                    direction.normalized,
                    direction.magnitude,
                    obstacleMask))
            {
                continue;
            }

            CurrentTarget =
                targetCollider.transform;

            return true;
        }

        CurrentTarget = null;
        return false;
    }

    private void LateUpdate()
    {
        if (!showDebugInGame)
        {
            return;
        }

        UpdateDebugLines();
    }

    private void CreateDebugLines()
    {
        fovCircle = CreateLine(
            "FOV Circle",
            fovColor,
            circleSegments + 1
        );

        leftLine = CreateLine(
            "FOV Left",
            fovColor,
            2
        );

        rightLine = CreateLine(
            "FOV Right",
            fovColor,
            2
        );

        targetLine = CreateLine(
            "FOV Target",
            targetColor,
            2
        );
    }

    private LineRenderer CreateLine(
        string lineName,
        Color color,
        int positions)
    {
        GameObject lineObject =
            new GameObject(lineName);

        lineObject.transform.SetParent(
            transform,
            false
        );

        LineRenderer line =
            lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = positions;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        line.startColor = color;
        line.endColor = color;

        return line;
    }

    private void UpdateDebugLines()
    {
        Vector3 center =
            transform.position + Vector3.up * 0.1f;

        float halfAngle =
            detectionAngle * 0.5f;

        Vector3 leftBoundary =
            Quaternion.Euler(
                0f,
                -halfAngle,
                0f
            ) * transform.forward;

        Vector3 rightBoundary =
            Quaternion.Euler(
                0f,
                halfAngle,
                0f
            ) * transform.forward;

        for (int i = 0; i <= circleSegments; i++)
        {
            float t =
                (float)i / circleSegments;

            float angle =
                -halfAngle +
                detectionAngle * t;

            Vector3 direction =
                Quaternion.Euler(
                    0f,
                    angle,
                    0f
                ) * transform.forward;

            fovCircle.SetPosition(
                i,
                center +
                direction * detectionRadius
            );
        }

        leftLine.SetPosition(0, center);
        leftLine.SetPosition(
            1,
            center +
            leftBoundary * detectionRadius
        );

        rightLine.SetPosition(0, center);
        rightLine.SetPosition(
            1,
            center +
            rightBoundary * detectionRadius
        );

        if (CurrentTarget != null)
        {
            targetLine.enabled = true;

            targetLine.SetPosition(
                0,
                center
            );

            targetLine.SetPosition(
                1,
                CurrentTarget.position
            );
        }
        else
        {
            targetLine.enabled = false;
        }
    }

    private void OnDestroy()
    {
        DestroyLine(fovCircle);
        DestroyLine(leftLine);
        DestroyLine(rightLine);
        DestroyLine(targetLine);
    }

    private void DestroyLine(LineRenderer line)
    {
        if (line != null)
        {
            if (line.material != null)
            {
                Destroy(line.material);
            }

            Destroy(line.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );

        Vector3 leftBoundary =
            Quaternion.Euler(
                0f,
                -detectionAngle * 0.5f,
                0f
            ) * transform.forward;

        Vector3 rightBoundary =
            Quaternion.Euler(
                0f,
                detectionAngle * 0.5f,
                0f
            ) * transform.forward;

        Gizmos.DrawRay(
            transform.position,
            leftBoundary * detectionRadius
        );

        Gizmos.DrawRay(
            transform.position,
            rightBoundary * detectionRadius
        );

        if (CurrentTarget != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                transform.position,
                CurrentTarget.position
            );
        }
    }
}