using UnityEngine;
using UnityEngine.AI;

public sealed class AssaultController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float attackRange = 2f;

    private NavMeshAgent _navMeshAgent;
    private EnemyFOV _enemyFOV;
    private StateMachine _stateMachine;

    public Transform CurrentTarget =>
        _enemyFOV != null ? _enemyFOV.CurrentTarget : null;

    public NavMeshAgent NavMeshAgent =>
        _navMeshAgent;

    public float AttackRange =>
        attackRange;

    public float ChaseSpeed =>
        chaseSpeed;

    private void Awake()
    {
        _navMeshAgent =
            GetComponent<NavMeshAgent>();

        _enemyFOV =
            GetComponent<EnemyFOV>();

        _stateMachine =
            new StateMachine();

        if (_navMeshAgent == null)
        {
            Debug.LogError(
                $"{name}: NavMeshAgent is missing.",
                this);
        }

        if (_enemyFOV == null)
        {
            Debug.LogError(
                $"{name}: EnemyFOV is missing.",
                this);
        }

        SetIdleState();
    }

    private void Update()
    {
        if (_stateMachine == null)
        {
            return;
        }

        _stateMachine.Tick();
    }

    public bool CanSeeTarget()
    {
        if (_enemyFOV == null)
        {
            return false;
        }

        return _enemyFOV.TryFindTarget();
    }

    public void SetIdleState()
    {
        _stateMachine.SetState(
            new AssaultIdleState(this));
    }

    public void SetChaseState()
    {
        _stateMachine.SetState(
            new AssaultChaseState(this));
    }

    public void SetAttackState()
    {
        _stateMachine.SetState(
            new AssaultAttackState(this));
    }

    public void SetRecoveryState()
    {
        _stateMachine.SetState(
            new AssaultRecoveryState(this));
    }

    public void SetBackstepState()
    {
        _stateMachine.SetState(
            new AssaultBackstepState(this));
    }
}