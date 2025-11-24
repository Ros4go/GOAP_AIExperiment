using UnityEngine;

[CreateAssetMenu(fileName = "Action_MoveToWeapon", menuName = "GOAP/Actions/Move To Weapon")]
public class MoveToWeapon : GOAPAction
{
    public override void OnEnable()
    {
        base.OnEnable();
        ActionName = "MoveToWeapon";
    }

    public override bool IsDone(AgentGOAP agent)
    {
        MovementController movementController = agent.GetComponent<MovementController>();
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();

        if (worldStateSensor?.WeaponObject == null || worldStateSensor.WeaponObject.IsPickedUp && worldStateSensor.WeaponObject.IsPickedUpBy != agent.gameObject)
        {
            Debug.LogWarning($"{agent.name} arrête MoveToWeapon car l'arme est déjà prise.");
            return true;
        }

        return movementController == null || movementController.HasArrived();
    }

    public override void PerformAction(AgentGOAP agent)
    {
        MovementController movementController = agent.GetComponent<MovementController>();
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();

        if (movementController != null && worldStateSensor != null && worldStateSensor.WeaponObject != null)
        {
            Debug.Log($"{agent.name} commence à se déplacer vers l'arme.");
            movementController.MoveTo(worldStateSensor.WeaponObject.transform.position);
        }
        else
        {
            Debug.LogWarning($"{agent.name}: Impossible de MoveToWeapon. Contrôleur ou arme manquante.");
        }
    }

    public override void AbortAction(AgentGOAP agent)
    {
        MovementController movementController = agent.GetComponent<MovementController>();
        movementController?.StopMovement();
        Debug.Log($"{agent.name} annule le déplacement vers l'arme.");
    }
}