using UnityEngine;

public sealed class AssaultBackstepState : IState
{
    private readonly AssaultController enemy;

    private float timer;

    public AssaultBackstepState(AssaultController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        timer = 0.5f;

        Transform target = enemy.CurrentTarget;

        if (target == null)
        {
            return;
        }

        Vector3 direction =
            enemy.transform.position - target.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();

            Vector3 destination =
                enemy.transform.position + direction * 2f;

            enemy.NavMeshAgent.SetDestination(destination);
        }
    }

    public void Tick()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            enemy.SetChaseState();
        }
    }

    public void Exit()
    {
        enemy.NavMeshAgent.ResetPath();
    }
}