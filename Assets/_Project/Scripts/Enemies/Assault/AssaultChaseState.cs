using UnityEngine;

public sealed class AssaultChaseState : IState
{
    private readonly AssaultController enemy;

    public AssaultChaseState(AssaultController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.NavMeshAgent.speed = enemy.ChaseSpeed;
    }

    public void Tick()
    {
        if (!enemy.CanSeeTarget())
        {
            enemy.SetIdleState();
            return;
        }

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

        if (distance <= enemy.AttackRange)
        {
            enemy.SetAttackState();
            return;
        }

        enemy.NavMeshAgent.SetDestination(target.position);
    }

    public void Exit()
    {
        enemy.NavMeshAgent.ResetPath();
    }
}