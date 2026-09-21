using UnityEngine;

public sealed class AssaultRecoveryState : IState
{
    private readonly AssaultController enemy;

    private float timer;
    private Vector3 lockedRotation;

    public AssaultRecoveryState(AssaultController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.NavMeshAgent.ResetPath();

        timer = 3f;

        lockedRotation = enemy.transform.eulerAngles;
    }

    public void Tick()
    {
        enemy.transform.eulerAngles = lockedRotation;

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            return;
        }

        if (enemy.CurrentTarget == null)
        {
            enemy.SetIdleState();
            return;
        }

        if (enemy.CanSeeTarget())
        {
            enemy.SetChaseState();
        }
        else
        {
            enemy.SetIdleState();
        }
    }

    public void Exit()
    {
    }
}