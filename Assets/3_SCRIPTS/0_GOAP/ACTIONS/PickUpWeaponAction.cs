using UnityEngine;

[CreateAssetMenu(fileName = "Action_PickUpWeapon", menuName = "GOAP/Actions/Pick Up Weapon")]
public class PickUpWeapon : GOAPAction
{
    public override void OnEnable()
    {
        base.OnEnable();
        ActionName = "PickUpWeapon";
    }

    public override bool IsDone(AgentGOAP agent)
    {
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();
        if (worldStateSensor == null || worldStateSensor.WeaponObject == null)
        {
            return !agent.WorldStateSensor.GetCurrentWorldState().Matches(EffectsBit);
        }
        return worldStateSensor.GetCurrentWorldState().TryGet(WorldStateKey.HasWeapon, out bool hasWeapon) && hasWeapon;
    }

    public override void PerformAction(AgentGOAP agent)
    {
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();
        if (worldStateSensor != null && worldStateSensor.WeaponObject != null)
        {
            if (worldStateSensor.WeaponObject.PickUp(agent.gameObject))
            {
                Debug.Log($"{agent.name} ramasse l'arme : {worldStateSensor.WeaponObject.ObjectName}.");
            }
            else
            {
                Debug.LogWarning($"{agent.name}: Impossible de ramasser l'arme (déjà ramassée ou autre).");
            }
        }
        else
        {
            Debug.LogWarning($"{agent.name}: Impossible de PickUpWeapon. Arme manquante ou objet non trouvé.");
        }
    }

    public override void AbortAction(AgentGOAP agent)
    {
        Debug.Log($"{agent.name} annule de PickUpWeapon.");
    }
}