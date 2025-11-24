using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Action_SmokeCigarette", menuName = "GOAP/Actions/Smoke Cigarette")]
public class SmokeCigarette : GOAPAction
{
    private float duration = 3f;

    private class SmokeState
    {
        public float startTime;
        public bool started;
    }

    private Dictionary<AgentGOAP, SmokeState> stateMap = new();

    public override void OnEnable()
    {
        base.OnEnable();
        ActionName = "SmokeCigarette";
        Cost = Random.Range(0.1f, 1f);
    }

    public override bool IsDone(AgentGOAP agent)
    {
        if (stateMap.TryGetValue(agent, out var state) && state.started)
        {
            if (Time.time >= state.startTime + duration)
            {
                agent.WorldStateSensor.SetSmokedRecently(duration);
                stateMap.Remove(agent); 
                return true;
            }
        }
        return false;
    }

    public override void PerformAction(AgentGOAP agent)
    {
        if (!stateMap.ContainsKey(agent))
        {
            var movement = agent.GetComponent<MovementController>();
            movement?.StopMovement();

            stateMap[agent] = new SmokeState
            {
                startTime = Time.time,
                started = true
            };

            Debug.Log($"{agent.name} s'allume une cigarette pour {duration} secondes.");
        }
    }

    public override void AbortAction(AgentGOAP agent)
    {
        if (stateMap.ContainsKey(agent))
        {
            stateMap.Remove(agent);
            Debug.Log($"{agent.name} interrompt sa cigarette.");
        }
    }
}
