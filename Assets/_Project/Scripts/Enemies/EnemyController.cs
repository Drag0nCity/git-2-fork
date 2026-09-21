using TPShooter.Player;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public sealed class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float coverSearchRadius = 8f;
    [SerializeField] private LayerMask coverMask;

    private EnemyWeapon enemyWeapon;
    private StateMachine stateMachine;
    private EnemyFOV enemyFOV;
    private NavMeshAgent navMeshAgent;
    private PlayerController playerController;
    public float FireCooldown => enemyWeapon.FireCooldown;
    public int BurstCount => enemyWeapon.BurstCount;
    public float BurstDelay => enemyWeapon.BurstDelay;
    public Vector3 TargetVelocity => playerController.Velocity;
    private EnemyHealth enemyHealth;
    private bool isDead;

    private int currentPatrolPointIndex;

    public Transform CurrentTarget => enemyFOV.CurrentTarget;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public float CurrentHealth => enemyHealth.CurrentHealth;
    public float MaxHealth => enemyHealth.MaxHealth;
    public EnemyWeapon EnemyWeapon => enemyWeapon;



    [Inject]
    private void Construct(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    private void Awake()
    {
        enemyWeapon = GetComponent<EnemyWeapon>();
        stateMachine = new StateMachine();
        enemyFOV = GetComponent<EnemyFOV>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.Died += OnDied;
        }

        SetPatrolState();
    }

    private void Update()
    {
        stateMachine.Tick();
    }

    public bool CanSeeTarget()
    {
        return enemyFOV.TryFindTarget();
    }

    public Transform GetCurrentPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return null;
        }

        return patrolPoints[currentPatrolPointIndex];
    }

    public void GoToNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        currentPatrolPointIndex++;

        if (currentPatrolPointIndex >= patrolPoints.Length)
        {
            currentPatrolPointIndex = 0;
        }
    }

    public void SetPatrolState()
    {
        stateMachine.SetState(
            new EnemyPatrolState(this)
        );
    }

    public void SetCombatState()
    {
        stateMachine.SetState(
            new EnemyCombatState(this)
        );
    }

    public void SetTakeCoverState()
    {
        stateMachine.SetState(
            new EnemyTakeCoverState(this)
        );
    }

    public void SetAlertState()
    {
        stateMachine.SetState(
            new EnemyAlertState(this)
        );
    }


    public void FireAtTarget()
    {
        if (isDead)
            return;

        if (enemyWeapon == null || CurrentTarget == null)
            return;

        enemyWeapon.Fire(CurrentTarget, TargetVelocity);
    }

    public bool TryFindCover(out Vector3 coverPosition)
    {
        coverPosition = transform.position;

        Transform target = CurrentTarget;

        if (target == null)
        {
            return false;
        }

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude <= 0.001f)
        {
            return false;
        }

        Vector3 directionToTarget = toTarget.normalized;

        Vector3[] directions =
        {
        -directionToTarget,
        Vector3.Cross(directionToTarget, Vector3.up),
        -Vector3.Cross(directionToTarget, Vector3.up),
        (-directionToTarget + Vector3.Cross(directionToTarget, Vector3.up)).normalized,
        (-directionToTarget - Vector3.Cross(directionToTarget, Vector3.up)).normalized
    };

        Vector3 rayOrigin = target.position + Vector3.up;

        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 candidate =
                transform.position + directions[i] * coverSearchRadius;

            Vector3 rayDirection = candidate - rayOrigin;

            if (Physics.Raycast(
                    rayOrigin,
                    rayDirection.normalized,
                    rayDirection.magnitude,
                    coverMask))
            {
                if (NavMeshAgent != null &&
                    NavMesh.SamplePosition(
                        candidate,
                        out NavMeshHit navMeshHit,
                        2f,
                        NavMesh.AllAreas))
                {
                    coverPosition = navMeshHit.position;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDied()
    {
        isDead = true;

        if (enemyFOV != null)
        {
            enemyFOV.enabled = false;
        }

        if (navMeshAgent != null &&
            navMeshAgent.isActiveAndEnabled &&
            navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        if (enemyWeapon != null)
        {
            enemyWeapon.enabled = false;
        }

        stateMachine.SetState(null);
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.Died -= OnDied;
        }
    }
}