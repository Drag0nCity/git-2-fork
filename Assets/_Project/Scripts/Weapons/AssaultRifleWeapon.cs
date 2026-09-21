using System.Collections;
using UnityEngine;

public sealed class AssaultRifleWeapon : WeaponBase
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LayerMask hitMask;

    [Header("Game View Shot")]
    [SerializeField] private bool showShotLine = true;
    [SerializeField] private Color shotColor = Color.yellow;
    [SerializeField] private float shotLineWidth = 0.025f;
    [SerializeField] private float shotDuration = 0.08f;

    private LineRenderer shotLine;
    private Coroutine hideShotRoutine;

    protected override void Awake()
    {
        base.Awake();

        if (showShotLine)
        {
            CreateShotLine();
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

        Vector3 origin = muzzlePoint.position;

        Vector3 direction = muzzlePoint.forward;

        direction +=
            muzzlePoint.right *
            Random.Range(-Spread, Spread);

        direction +=
            muzzlePoint.up *
            Random.Range(-Spread, Spread);

        direction.Normalize();

        Vector3 endPoint =
            origin + direction * Range;

        if (Physics.Raycast(
                origin,
                direction,
                out RaycastHit hit,
                Range,
                hitMask))
        {
            endPoint = hit.point;

            IDamageable damageable =
                hit.collider.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(Damage);
            }
        }

        ShowShotLine(
            origin,
            endPoint);
    }

    private void CreateShotLine()
    {
        GameObject lineObject =
            new GameObject(
                "Assault Rifle Shot Line");

        lineObject.transform.SetParent(
            transform,
            false);

        shotLine =
            lineObject.AddComponent<LineRenderer>();

        shotLine.useWorldSpace = true;
        shotLine.positionCount = 2;

        shotLine.startWidth = shotLineWidth;
        shotLine.endWidth = shotLineWidth;

        shotLine.material =
            new Material(
                Shader.Find("Sprites/Default"));

        shotLine.startColor = shotColor;
        shotLine.endColor = shotColor;

        shotLine.enabled = false;
    }

    private void ShowShotLine(
        Vector3 start,
        Vector3 end)
    {
        if (shotLine == null)
        {
            return;
        }

        shotLine.SetPosition(0, start);
        shotLine.SetPosition(1, end);

        shotLine.enabled = true;

        if (hideShotRoutine != null)
        {
            StopCoroutine(hideShotRoutine);
        }

        hideShotRoutine =
            StartCoroutine(
                HideShotLine());
    }

    private IEnumerator HideShotLine()
    {
        yield return new WaitForSeconds(
            shotDuration);

        if (shotLine != null)
        {
            shotLine.enabled = false;
        }

        hideShotRoutine = null;
    }

    private void OnDestroy()
    {
        if (shotLine != null)
        {
            if (shotLine.material != null)
            {
                Destroy(shotLine.material);
            }

            Destroy(shotLine.gameObject);
        }
    }
}