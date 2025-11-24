using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Action_HealSelf", menuName = "GOAP/Actions/Heal Self")]
public class HealSelf : GOAPAction
{
    private float healDuration = 1f;
    private float startTime;
    private bool hasStarted = false;

    public override void OnEnable()
    {
        base.OnEnable();
        ActionName = "HealSelf";
    }

    public override bool IsDone(AgentGOAP agent)
    {
        if (hasStarted && Time.time >= startTime + healDuration)
        {
            var health = agent.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.Heal(30); 
                Debug.Log($"{agent.name} termine le soin.");
            }
            return true;
        }
        return false;
    }

    public override void PerformAction(AgentGOAP agent)
    {
        if (!hasStarted)
        {
            startTime = Time.time;
            hasStarted = true;
            Debug.Log($"{agent.name} commence à se soigner (pendant {healDuration} secondes).");
        }
    }

    public override void AbortAction(AgentGOAP agent)
    {
        Debug.Log($"{agent.name} interrompt son soin.");
        hasStarted = false;
    }
}
