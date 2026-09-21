using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyCombatState : IState
{
    private readonly EnemyController controller;

    private CancellationTokenSource cancellationTokenSource;

    private float burstDelay = 0.25f;
    private float timeBetweenBursts = 1.2f;

    public EnemyCombatState(EnemyController controller)
    {
        this.controller = controller;
    }

    public void Enter()
    {
        StopTasks();

        if (controller == null)
            return;

        NavMeshAgent agent = controller.NavMeshAgent;

        if (agent != null &&
            agent.isActiveAndEnabled &&
            agent.isOnNavMesh)
        {
            agent.ResetPath();
            agent.isStopped = true;
        }

        cancellationTokenSource =
            new CancellationTokenSource();

        StartBurstAsync(
            cancellationTokenSource.Token
        ).Forget();
    }

    public void Tick()
    {
        if (controller == null)
            return;

        NavMeshAgent agent = controller.NavMeshAgent;

        if (agent != null &&
            agent.isActiveAndEnabled &&
            agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        if (!controller.CanSeeTarget())
        {
            controller.SetPatrolState();
            return;
        }

        Transform target = controller.CurrentTarget;

        if (target == null)
        {
            controller.SetPatrolState();
            return;
        }

        Vector3 direction =
            target.position -
            controller.transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            controller.transform.rotation =
                Quaternion.Slerp(
                    controller.transform.rotation,
                    targetRotation,
                    Time.deltaTime * 8f
                );
        }
    }

    public void Exit()
    {
        StopTasks();

        if (controller == null)
            return;

        NavMeshAgent agent = controller.NavMeshAgent;

        if (agent != null &&
            agent.isActiveAndEnabled &&
            agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    private async UniTaskVoid StartBurstAsync(
        CancellationToken token)
    {
        try
        {
            await UniTask.Delay(
                250,
                cancellationToken: token
            );

            while (!token.IsCancellationRequested)
            {
                if (controller == null)
                    return;

                if (controller.CurrentTarget == null)
                    return;

                controller.FireAtTarget();

                await UniTask.Delay(
                    Mathf.RoundToInt(
                        burstDelay * 1000f
                    ),
                    cancellationToken: token
                );

                if (token.IsCancellationRequested)
                    return;

                if (controller == null)
                    return;

                if (controller.CurrentTarget == null)
                    return;

                controller.FireAtTarget();

                await UniTask.Delay(
                    Mathf.RoundToInt(
                        burstDelay * 1000f
                    ),
                    cancellationToken: token
                );

                if (token.IsCancellationRequested)
                    return;

                if (controller == null)
                    return;

                if (controller.CurrentTarget == null)
                    return;

                controller.FireAtTarget();

                await UniTask.Delay(
                    Mathf.RoundToInt(
                        timeBetweenBursts * 1000f
                    ),
                    cancellationToken: token
                );
            }
        }
        catch (OperationCanceledException)
        {
            // Нормальная отмена при выходе из состояния.
        }
    }

    private void StopTasks()
    {
        if (cancellationTokenSource == null)
            return;

        if (!cancellationTokenSource.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
        }

        cancellationTokenSource.Dispose();
        cancellationTokenSource = null;
    }
}