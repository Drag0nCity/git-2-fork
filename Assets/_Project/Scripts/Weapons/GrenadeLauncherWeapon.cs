using System.Collections;
using UnityEngine;

public sealed class GrenadeLauncherWeapon : WeaponBase
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private float spawnOffset = 0.8f;
    [SerializeField] private float reloadTime = 1.5f;

    private bool isReloading;

    public override void Fire()
    {
        if (isReloading)
        {
            return;
        }

        if (muzzlePoint == null)
        {
            Debug.LogError(
                $"{name}: Muzzle Point is not assigned.",
                this);

            return;
        }

        if (Config == null)
        {
            return;
        }

        if (Config.ProjectilePrefab == null)
        {
            Debug.LogError(
                $"{name}: Projectile Prefab is not assigned.",
                this);

            return;
        }

        Vector3 spawnPosition =
            muzzlePoint.position +
            muzzlePoint.forward * spawnOffset;

        GameObject projectile =
            Instantiate(
                Config.ProjectilePrefab,
                spawnPosition,
                muzzlePoint.rotation);

        GrenadeProjectile grenade =
            projectile.GetComponent<GrenadeProjectile>();

        if (grenade == null)
        {
            Destroy(projectile);
            return;
        }

        grenade.IgnoreOwner(
            transform.root.gameObject);

        grenade.Launch(
            muzzlePoint.forward);
    }

    public override void Reload()
    {
        if (isReloading)
        {
            return;
        }

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        isReloading = false;
    }
}