using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionScheduler : MonoBehaviour
{
    private Queue<GOAPAction> _planQueue = new Queue<GOAPAction>();

    private GOAPAction _currentAction;
    private AgentGOAP _agent;

    public GOAPAction CurrentAction => _currentAction;

    public void ExecutePlan(Queue<GOAPAction> plan, AgentGOAP agent)
    {
        if (plan == null || !plan.Any())
        {
            Debug.LogWarning("ActionScheduler: Plan vide ou nul reçu. Arrêt de l'exécution.");
            AbortCurrentAction(agent);
            return;
        }

        _planQueue = plan;
        _agent = agent;
        StartNextAction();
    }

    protected virtual void Update()
    {
        if (_agent == null) return;

        if (_currentAction == null)
        {
            if (_planQueue.Any())
            {
                StartNextAction();
            }
            return;
        }

        if (_currentAction.IsDone(_agent))
        {
            Debug.Log($"[DEBUG] État courant : {_agent.WorldStateSensor.GetCurrentWorldState()}");
            Debug.Log($"[DEBUG] Effets attendus de l'action '{_currentAction.ActionName}' : {_currentAction.EffectsBit}");
            Debug.Log($"[DEBUG] Match ? {_agent.WorldStateSensor.GetCurrentWorldState().Matches(_currentAction.EffectsBit)}");
            bool actionActuallySucceeded = _agent.WorldStateSensor.GetCurrentWorldState().Matches(_currentAction.EffectsBit);

            Debug.Log($"ActionScheduler: Action '{_currentAction.ActionName}' {(actionActuallySucceeded ? "terminée avec succès" : "potentiellement échouée")} pour {_agent.name}.");
            GOAPAction finishedAction = _currentAction;
            _currentAction = null;

            if (actionActuallySucceeded)
            {
                _agent.NotifyActionCompletion(finishedAction, true);
            }
            else
            {
                _agent.NotifyActionCompletion(finishedAction, false);
                AbortCurrentAction(_agent);
            }

            if (actionActuallySucceeded && _planQueue.Any())
            {
                StartNextAction();
            }
        }
    }

    public void AbortCurrentAction(AgentGOAP agent)
    {
        if (_currentAction != null)
        {
            Debug.Log($"ActionScheduler: Annulation de l'action '{_currentAction.ActionName}' pour {agent.name}.");
            _currentAction.AbortAction(agent);
            _currentAction = null;
        }
    }

    private void StartNextAction()
    {
        if (!_planQueue.Any())
        {
            Debug.Log($"ActionScheduler: Plan terminé pour {_agent.name}.");
            _currentAction = null;
            return;
        }

        _currentAction = _planQueue.Dequeue();
        Debug.Log($"ActionScheduler: Démarrage de l'action '{_currentAction.ActionName}' pour {_agent.name}.");

        try
        {
            _currentAction.PerformAction(_agent);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ActionScheduler: Erreur lors de l'exécution de l'action '{_currentAction.ActionName}' pour {_agent.name}: {e.Message}");
            AbortCurrentAction(_agent);
            _agent.NotifyActionCompletion(_currentAction, false);
        }
    }
}