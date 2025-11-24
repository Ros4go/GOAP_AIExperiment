using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentGOAP : MonoBehaviour
{
    public WorldStateSensor WorldStateSensor;
    public ActionScheduler ActionScheduler;

    public GOAPAction[] AvailableActions;

    public float ReplanInterval = 1.0f;

    public WorldStateBit GoalState = new WorldStateBit();
    public WorldStateBit offensiveGoal;
    public WorldStateBit defensiveGoal;

    private GOAPPlanner _planner;
    private Queue<GOAPAction> _currentPlan;
    private float _nextReplanTime;

    private bool isInDefensiveMode = false;
    public float LastPlanTimeMs = 0f;

    public Queue<GOAPAction> GetCurrentPlan()
    {
        return _currentPlan != null ? new Queue<GOAPAction>(_currentPlan) : new Queue<GOAPAction>();
    }

    protected virtual void Awake()
    {
        _planner = new GOAPPlanner();
        _currentPlan = new Queue<GOAPAction>();

        if (WorldStateSensor == null) WorldStateSensor = GetComponent<WorldStateSensor>();
        if (ActionScheduler == null) ActionScheduler = GetComponent<ActionScheduler>();
    }

    protected virtual void Start()
    {
        offensiveGoal = new WorldStateBit();
        offensiveGoal.Set(WorldStateKey.IsEnemyDead, true);
        offensiveGoal.Set(WorldStateKey.NeedsToRelax, false);

        defensiveGoal = new WorldStateBit();
        defensiveGoal.Set(WorldStateKey.ShouldFlee, false);
        defensiveGoal.Set(WorldStateKey.IsHurt, false);
        defensiveGoal.Set(WorldStateKey.NeedsToRelax, false);

        GoalState = offensiveGoal.Clone();

        _nextReplanTime = Time.time + ReplanInterval;
        PlanAndExecute();
    }

    protected virtual void Update()
    {

        WorldStateBit currentState = WorldStateSensor.GetCurrentWorldState();

        bool shouldDefend = currentState.TryGet(WorldStateKey.ShouldFlee, out bool flee) && flee;
        shouldDefend |= currentState.TryGet(WorldStateKey.IsHurt, out bool hurt) && hurt;

        if (shouldDefend && !isInDefensiveMode)
        {
            isInDefensiveMode = true;
            Debug.Log($"{name}: passe en mode défensif.");
            SetGoal(defensiveGoal);
        }
        else if (!shouldDefend && isInDefensiveMode)
        {
            isInDefensiveMode = false;
            Debug.Log($"{name}: repasse en mode offensif.");
            SetGoal(offensiveGoal);
        }

        if (currentState.Matches(GoalState))
        {
            if (_currentPlan.Any())
            {
                ActionScheduler.AbortCurrentAction(this);
                _currentPlan.Clear();
                Debug.Log($"GOAP: Goal '{GoalState}' already achieved for {gameObject.name}. Clearing plan.");
            }
            return;
        }

        if (ReplanInterval > 0 && Time.time >= _nextReplanTime)
        {
            Debug.Log($"GOAP: Replanification périodique pour {gameObject.name}...");
            PlanAndExecute();
            _nextReplanTime = Time.time + ReplanInterval;
        }

        if (!_currentPlan.Any() && !currentState.Matches(GoalState))
        {
            Debug.Log($"GOAP: Plan terminé ou invalide pour {gameObject.name}. Replanification...");
            PlanAndExecute();
        }
    }

    public void SetGoal(WorldStateBit  newGoal)
    {
        GoalState = newGoal;
        Debug.Log($"GOAP: Nouveau but défini pour {gameObject.name}: {newGoal}. Replanification immédiate.");
        PlanAndExecute();
    }

    private void PlanAndExecute()
    {
        WorldStateBit  currentState = WorldStateSensor.GetCurrentWorldState();

        Queue<GOAPAction> newPlan = _planner.Plan(this, currentState, GoalState, new HashSet<GOAPAction>(AvailableActions));

        if (newPlan != null && newPlan.Any())
        {
            _currentPlan = newPlan;
            Debug.Log($"GOAP: Nouveau plan trouvé pour {gameObject.name}. Exécution du plan avec {newPlan.Count} actions.");
            foreach (var action in _currentPlan)
            {
                Debug.Log($"- {action.ActionName}");
            }
            ActionScheduler.ExecutePlan(_currentPlan, this);
        }
        else
        {
            _currentPlan.Clear();
            Debug.LogWarning($"GOAP: Impossible de trouver un plan pour {gameObject.name} avec le but {GoalState}.");
            ActionScheduler.AbortCurrentAction(this);
        }
    }

    public void NotifyActionCompletion(GOAPAction completedAction, bool success)
    {
        if (success)
        {
            Debug.Log($"GOAP: Action '{completedAction.ActionName}' terminée avec succès pour {gameObject.name}.");
        }
        else
        {
            Debug.LogWarning($"GOAP: Action '{completedAction.ActionName}' échouée pour {gameObject.name}. Replanification immédiate.");
            ActionScheduler.AbortCurrentAction(this);
            _currentPlan.Clear();
            PlanAndExecute();
        }
    }
}