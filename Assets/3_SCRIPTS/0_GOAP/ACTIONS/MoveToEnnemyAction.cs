using UnityEngine;

[CreateAssetMenu(fileName = "Action_MoveToEnemy", menuName = "GOAP/Actions/Move To Enemy")]
public class MoveToEnemy : GOAPAction
{
    public override void OnEnable()
    {
        base.OnEnable();
        ActionName = "MoveToEnemy";
    }

    public override bool IsDone(AgentGOAP agent)
    {
        MovementController movementController = agent.GetComponent<MovementController>();
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();

        if (movementController == null || worldStateSensor == null) return true; 

        bool hasArrivedByMovement = movementController.HasArrived();
        bool isNearEnemyInWorldState = worldStateSensor.GetCurrentWorldState().TryGet(WorldStateKey.IsNearEnemy, out bool isNear) && isNear;

        return hasArrivedByMovement && isNearEnemyInWorldState;
    }

    public override void PerformAction(AgentGOAP agent)
    {
        MovementController movementController = agent.GetComponent<MovementController>();
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();

        if (movementController != null && worldStateSensor != null && worldStateSensor.EnemyTransform != null)
        {
            Debug.Log($"{agent.name} commence à se déplacer vers l'ennemi.");
            movementController.MoveToTarget(worldStateSensor.EnemyTransform);
        }
        else
        {
            Debug.LogWarning($"{agent.name}: Impossible de MoveToEnemy. Contrôleur ou ennemi manquant.");
        }
    }

    public override void AbortAction(AgentGOAP agent)
    {
        MovementController movementController = agent.GetComponent<MovementController>();
        movementController?.StopMovement();
        Debug.Log($"{agent.name} annule le déplacement vers l'ennemi.");
    }
}