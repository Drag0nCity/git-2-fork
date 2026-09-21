using System.Collections;
using UnityEngine;

public class PistolWeapon : WeaponBase
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LayerMask hitMask;

    [Header("Game View Shot")]
    [SerializeField] private bool showShotLine = true;
    [SerializeField] private Color shotColor = Color.cyan;
    [SerializeField] private float shotLineWidth = 0.025f;
    [SerializeField] private float shotDuration = 0.08f;

    private LineRenderer shotLine;
    private Coroutine hideShotRoutine;

    private int currentMagazineAmmo;

    protected override void Awake()
    {
        base.Awake();

        currentMagazineAmmo = MagazineSize;

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

        if (currentMagazineAmmo <= 0)
        {
            Debug.Log("Pistol magazine is empty.");
            return;
        }

        currentMagazineAmmo--;

        Vector3 origin = muzzlePoint.position;
        Vector3 direction = muzzlePoint.forward;

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

        ShowShotLine(origin, endPoint);
    }

    public override void Reload()
    {
        if (currentMagazineAmmo >= MagazineSize)
        {
            return;
        }

        if (CurrentReserveAmmo <= 0)
        {
            return;
        }

        int neededAmmo =
            MagazineSize - currentMagazineAmmo;

        int ammoToLoad =
            Mathf.Min(
                neededAmmo,
                CurrentReserveAmmo);

        currentMagazineAmmo += ammoToLoad;

        AddAmmo(-ammoToLoad);

        Debug.Log(
            $"Pistol reloaded: {currentMagazineAmmo}/{MagazineSize}");
    }

    public int CurrentMagazineAmmo =>
        currentMagazineAmmo;

    private void CreateShotLine()
    {
        GameObject lineObject =
            new GameObject("Pistol Shot Line");

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