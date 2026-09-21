using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public sealed class EnemyTakeCoverState : IState
{
    private readonly EnemyController enemy;
    private bool isReloading;
    private CancellationTokenSource reloadCancellation;

    public EnemyTakeCoverState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log($"{enemy.name}: TakeCover");

        enemy.NavMeshAgent.ResetPath();
    }

    public void Tick()
    {
        Transform target = enemy.CurrentTarget;

        if (target == null)
        {
            enemy.SetPatrolState();
            return;
        }

        if (enemy.CurrentHealth > enemy.MaxHealth * 0.3f)
        {
            enemy.SetCombatState();
            return;
        }

        if (!isReloading && enemy.EnemyWeapon.CurrentAmmo < enemy.EnemyWeapon.MagazineSize)
        {
            Reload();
            return;
        }

        if (enemy.TryFindCover(out Vector3 coverPosition))
        {
            enemy.NavMeshAgent.SetDestination(coverPosition);
        }
        else
        {
            Vector3 directionAwayFromTarget =
                enemy.transform.position - target.position;

            directionAwayFromTarget.y = 0f;

            if (directionAwayFromTarget.sqrMagnitude > 0.001f)
            {
                directionAwayFromTarget.Normalize();

                Vector3 fallbackPosition =
                    enemy.transform.position +
                    directionAwayFromTarget * 5f;

                enemy.NavMeshAgent.SetDestination(fallbackPosition);
            }
        }
    }

    private void Reload()
    {
        if (isReloading)
        {
            return;
        }

        isReloading = true;

        reloadCancellation?.Cancel();
        reloadCancellation?.Dispose();

        reloadCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            enemy.GetCancellationTokenOnDestroy()
        );

        ReloadAsync(reloadCancellation.Token).Forget();
    }

    private async UniTask ReloadAsync(CancellationToken token)
    {
        try
        {
            await enemy.EnemyWeapon.ReloadAsync()
                .AttachExternalCancellation(token);

            if (token.IsCancellationRequested)
            {
                return;
            }

            isReloading = false;

            if (enemy.CurrentTarget != null)
            {
                enemy.SetCombatState();
            }
        }
        catch (System.OperationCanceledException)
        {
            isReloading = false;
        }
    }
    public void Exit()
    {
        reloadCancellation?.Cancel();
        reloadCancellation?.Dispose();
        reloadCancellation = null;

        isReloading = false;
    }
}