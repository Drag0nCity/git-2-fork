using UnityEngine;

public sealed class AssaultIdleState : IState
{
    private readonly AssaultController enemy;

    public AssaultIdleState(AssaultController enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.NavMeshAgent.ResetPath();
    }

    public void Tick()
    {
        if (enemy.CanSeeTarget())
        {
            enemy.SetChaseState();
        }
    }

    public void Exit()
    {
    }
}