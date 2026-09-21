using UnityEngine;

public sealed class EnemyPatrolState : IState
{
    private readonly EnemyController enemy;

    public EnemyPatrolState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log($"{enemy.name}: Patrol");

        MoveToCurrentPoint();
    }

    public void Tick()
    {
        if (enemy.CanSeeTarget())
        {
            enemy.SetAlertState();
            return;
        }

        Transform patrolPoint = enemy.GetCurrentPatrolPoint();

        if (patrolPoint == null)
        {
            return;
        }

        if (!enemy.NavMeshAgent.pathPending &&
            enemy.NavMeshAgent.remainingDistance <=
            enemy.NavMeshAgent.stoppingDistance)
        {
            enemy.GoToNextPatrolPoint();
            MoveToCurrentPoint();
        }
    }

    public void Exit()
    {
        enemy.NavMeshAgent.ResetPath();
    }

    private void MoveToCurrentPoint()
    {
        Transform patrolPoint = enemy.GetCurrentPatrolPoint();

        if (patrolPoint == null)
        {
            return;
        }

        enemy.NavMeshAgent.SetDestination(
            patrolPoint.position
        );
    }
}