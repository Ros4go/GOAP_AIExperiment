using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Action_FleeFromEnemy", menuName = "GOAP/Actions/Flee From Enemy")]
public class FleeFromEnemy : GOAPAction
{
    public float fleeRadius = 10f;
    public float fleeDuration = 2f;

    private class FleeData
    {
        public Vector3 target;
        public float startTime;
        public bool started;
    }

    private Dictionary<AgentGOAP, FleeData> _fleeStates = new();

    public override void OnEnable()
    {
        base.OnEnable();
        ActionName = "FleeFromEnemy";
    }

    public override bool IsDone(AgentGOAP agent)
    {
        if (!_fleeStates.ContainsKey(agent)) return false;

        var data = _fleeStates[agent];
        return Time.time >= data.startTime + fleeDuration;
    }

    public override void PerformAction(AgentGOAP agent)
    {
        if (_fleeStates.ContainsKey(agent) && _fleeStates[agent].started)
            return;

        var movement = agent.GetComponent<MovementController>();
        if (movement != null)
        {
            Vector3 randomDirection = Random.insideUnitSphere * fleeRadius + agent.transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, fleeRadius, NavMesh.AllAreas))
            {
                Debug.Log($"{agent.name} commence à fuir !");
                movement.MoveTo(hit.position);
                Debug.Log($"{agent.name} fuit vers {hit.position}");

                _fleeStates[agent] = new FleeData
                {
                    target = hit.position,
                    startTime = Time.time,
                    started = true
                };
            }
            else
            {
                Debug.LogWarning($"{agent.name} n'a pas pu trouver un point de fuite valide.");
            }
        }
    }

    public override void AbortAction(AgentGOAP agent)
    {
        agent.GetComponent<MovementController>()?.StopMovement();
        Debug.Log($"{agent.name} annule l'action de fuite.");
        _fleeStates.Remove(agent);
    }
}
