using UnityEngine;

public sealed class EnemyAlertState : IState
{
    private readonly EnemyController enemy;

    public EnemyAlertState(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log($"{enemy.name}: Alert");
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

        Vector3 direction = target.position - enemy.transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
        }

        if (!enemy.CanSeeTarget())
        {
            enemy.SetPatrolState();
            return;
        }

        enemy.SetCombatState();
    }

    public void Exit()
    {
    }
}