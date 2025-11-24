using UnityEngine;

[CreateAssetMenu(fileName = "Action_KillEnemy", menuName = "GOAP/Actions/Kill Enemy")]
public class KillEnemy : GOAPAction
{
    public override void OnEnable()
    {
        base.OnEnable();
        ActionName = "KillEnemy";
    }

    public override bool IsDone(AgentGOAP agent)
    {
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();
        if (worldStateSensor != null)
        {
            bool isEnemyDead;
            if (worldStateSensor.GetCurrentWorldState().TryGet(WorldStateKey.IsEnemyDead, out isEnemyDead))
            {
                return isEnemyDead;
            }
        }
        return false;
    }

    public override void PerformAction(AgentGOAP agent)
    {
        AttackController attackController = agent.GetComponent<AttackController>();
        WorldStateSensor worldStateSensor = agent.GetComponent<WorldStateSensor>();

        if (attackController == null || worldStateSensor == null || worldStateSensor.EnemyTransform == null)
        {
            Debug.LogWarning($"{agent.name}: Impossible de KillEnemy. Contrôleur ou ennemi manquant.");
            return;
        }

        bool hasWeapon;
        if (!worldStateSensor.GetCurrentWorldState().TryGet(WorldStateKey.HasWeapon, out hasWeapon) || !hasWeapon)
        {
            Debug.LogWarning($"{agent.name}: N'a pas d'arme pour KillEnemy. L'action échoue.");
            return;
        }

        attackController.SetAttackTarget(worldStateSensor.EnemyTransform.gameObject);
        if (attackController.IsReadyToAttack())
        {
            if (attackController.TryAttack(worldStateSensor.EnemyTransform.gameObject))
            {
                Debug.Log($"{agent.name} attaque l'ennemi.");
            }
            else
            {
                Debug.LogWarning($"{agent.name}: Impossible d'attaquer l'ennemi (hors portée ou autre échec d'AttackController).");
            }
        }
    }

    public override void AbortAction(AgentGOAP agent)
    {
        AttackController attackController = agent.GetComponent<AttackController>();
        if (attackController != null)
        {
            attackController.SetAttackTarget(null);
        }
        Debug.Log($"{agent.name} annule de KillEnemy.");
    }
}