using UnityEngine;

public sealed class AssaultAttackState : IState
{
    private readonly AssaultController enemy;

    private float attackTimer;

    public AssaultAttackState(AssaultController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.NavMeshAgent.ResetPath();
        attackTimer = 0f;
    }

    public void Tick()
    {
        Transform target = enemy.CurrentTarget;

        if (target == null)
        {
            enemy.SetIdleState();
            return;
        }

        float distance =
            Vector3.Distance(
                enemy.transform.position,
                target.position
            );

        if (distance > enemy.AttackRange)
        {
            enemy.SetChaseState();
            return;
        }

        Vector3 direction =
            target.position - enemy.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            enemy.transform.rotation =
                Quaternion.Slerp(
                    enemy.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 10f
                );
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            Attack(target);

            enemy.SetRecoveryState();
        }
    }

    private void Attack(Transform target)
    {
        IDamageable damageable =
            target.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(20f);
        }
    }

    public void Exit()
    {
    }
}