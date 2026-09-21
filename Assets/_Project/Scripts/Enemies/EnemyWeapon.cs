using TPShooter.Player;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;


public sealed class EnemyWeapon : MonoBehaviour
{
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 50f;
    [SerializeField] private float fireCooldown = 1f;
    [SerializeField] private int burstCount = 3;
    [SerializeField] private float burstDelay = 0.15f;
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float aimSpread = 3f;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 1.5f;

    public int BurstCount => burstCount;
    public float BurstDelay => burstDelay;

    public int CurrentAmmo { get; private set; }
    public int MagazineSize => magazineSize;
    public float ReloadTime => reloadTime;
    public float FireCooldown => fireCooldown;
    private CancellationToken cancellationToken;
    private bool isReloading;

    private void Awake()
    {
        CurrentAmmo = magazineSize;
        cancellationToken = this.GetCancellationTokenOnDestroy();
    }
    public async UniTask ReloadAsync()
    {
        if (isReloading || CurrentAmmo >= magazineSize)
        {
            return;
        }

        isReloading = true;

        try
        {
            await UniTask.Delay(
              System.TimeSpan.FromSeconds(reloadTime),
                cancellationToken: cancellationToken
            );

            CurrentAmmo = magazineSize;
        }
        catch (System.OperationCanceledException)
        {
            // Задача отменена при уничтожении оружия.
        }
        finally
        {
            isReloading = false;
        }
    }
    public void Fire(Transform target, Vector3 targetVelocity)
    {
        if (muzzlePoint == null || target == null)
            return;

        if (CurrentAmmo <= 0)
        {
            return;
        }

        CurrentAmmo--;

        float distanceToTarget =
      Vector3.Distance(muzzlePoint.position, target.position);

        float flightTime =
            distanceToTarget / projectileSpeed;

        Vector3 predictedPosition =
            target.position + targetVelocity * flightTime;

        Vector3 direction =
      predictedPosition - muzzlePoint.position;

        direction.Normalize();

        direction = Quaternion.Euler(
            Random.Range(-aimSpread, aimSpread),
            Random.Range(-aimSpread, aimSpread),
            0f
        ) * direction;

        if (Physics.Raycast(
            muzzlePoint.position,
            direction,
            out RaycastHit hit,
            range,
            hitMask))
        {
            PlayerHealth playerHealth =
                hit.collider.GetComponentInParent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}