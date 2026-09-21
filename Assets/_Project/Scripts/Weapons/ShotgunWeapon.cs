using System.Collections;
using UnityEngine;

public sealed class ShotgunWeapon : WeaponBase
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LayerMask hitMask;

    [Header("Game View Shot")]
    [SerializeField] private bool showShotLines = true;
    [SerializeField] private Color shotColor = Color.cyan;
    [SerializeField] private float shotLineWidth = 0.025f;
    [SerializeField] private float shotDuration = 0.08f;

    private readonly RaycastHit[] _hitBuffer =
        new RaycastHit[16];

    private LineRenderer[] shotLines;
    private Coroutine hideShotRoutine;

    protected override void Awake()
    {
        base.Awake();

        if (showShotLines && Config != null)
        {
            CreateShotLines();
        }
    }

    public override void Fire()
    {
        if (muzzlePoint == null)
        {
            Debug.LogError(
                $"{name}: Muzzle Point is not assigned.",
                this);

            return;
        }

        int pelletCount = Config.PelletCount;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = muzzlePoint.forward;

            direction +=
                muzzlePoint.right *
                Random.Range(-Spread, Spread);

            direction +=
                muzzlePoint.up *
                Random.Range(-Spread, Spread);

            direction.Normalize();

            Vector3 origin = muzzlePoint.position;

            Vector3 endPoint =
                origin + direction * Range;

            int hitCount = Physics.RaycastNonAlloc(
                origin,
                direction,
                _hitBuffer,
                Range,
                hitMask);

            float closestDistance = Range;

            for (int j = 0; j < hitCount; j++)
            {
                if (_hitBuffer[j].collider == null)
                {
                    continue;
                }

                if (_hitBuffer[j].distance < closestDistance)
                {
                    closestDistance =
                        _hitBuffer[j].distance;

                    endPoint =
                        _hitBuffer[j].point;
                }

                IDamageable damageable =
                    _hitBuffer[j].collider
                        .GetComponentInParent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(Damage);
                }
            }

            if (showShotLines &&
                shotLines != null &&
                i < shotLines.Length)
            {
                shotLines[i].SetPosition(0, origin);
                shotLines[i].SetPosition(1, endPoint);
                shotLines[i].enabled = true;
            }
        }

        if (showShotLines)
        {
            if (hideShotRoutine != null)
            {
                StopCoroutine(hideShotRoutine);
            }

            hideShotRoutine =
                StartCoroutine(
                    HideShotLines());
        }
    }

    private void CreateShotLines()
    {
        shotLines =
            new LineRenderer[Config.PelletCount];

        for (int i = 0; i < shotLines.Length; i++)
        {
            GameObject lineObject =
                new GameObject(
                    $"Shotgun Shot Line {i}");

            lineObject.transform.SetParent(
                transform,
                false);

            LineRenderer line =
                lineObject.AddComponent<LineRenderer>();

            line.useWorldSpace = true;
            line.positionCount = 2;

            line.startWidth = shotLineWidth;
            line.endWidth = shotLineWidth;

            line.material =
                new Material(
                    Shader.Find("Sprites/Default"));

            line.startColor = shotColor;
            line.endColor = shotColor;

            line.enabled = false;

            shotLines[i] = line;
        }
    }

    private IEnumerator HideShotLines()
    {
        yield return new WaitForSeconds(
            shotDuration);

        if (shotLines != null)
        {
            for (int i = 0; i < shotLines.Length; i++)
            {
                if (shotLines[i] != null)
                {
                    shotLines[i].enabled = false;
                }
            }
        }

        hideShotRoutine = null;
    }

    private void OnDestroy()
    {
        if (shotLines == null)
        {
            return;
        }

        for (int i = 0; i < shotLines.Length; i++)
        {
            if (shotLines[i] == null)
            {
                continue;
            }

            if (shotLines[i].material != null)
            {
                Destroy(shotLines[i].material);
            }

            Destroy(shotLines[i].gameObject);
        }
    }
}