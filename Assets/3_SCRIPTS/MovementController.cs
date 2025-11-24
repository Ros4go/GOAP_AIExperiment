using UnityEngine;
using UnityEngine.AI;

public class MovementController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Transform _targetTransform;
    private Vector3 _fixedDestination;

    [SerializeField] private float _stoppingDistance = 0.5f;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if (_navMeshAgent != null)
        {
            _navMeshAgent.stoppingDistance = _stoppingDistance;
        }
    }

    private void Update()
    {
        if (_targetTransform != null && _navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.SetDestination(_targetTransform.position);
        }
    }

    public void MoveTo(Vector3 destination)
    {
        _targetTransform = null;
        _fixedDestination = destination;
        if (_navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(destination);
        }
    }

    public void MoveToTarget(Transform target)
    {
        _targetTransform = target;
        _fixedDestination = Vector3.zero;
        if (_navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_targetTransform.position);
        }
    }

    public void StopMovement()
    {
        _targetTransform = null;
        if (_navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
        }
    }

    public bool HasArrived()
    {
        if (_navMeshAgent == null || !_navMeshAgent.enabled || !_navMeshAgent.isOnNavMesh) return true; 

        if (_navMeshAgent.pathPending) return false;

        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + 0.01f) 
        {
            if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude < 0.01f)
            {
                return true;
            }
        }
        return false;
    }
}