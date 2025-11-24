using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class AIDebugDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private Camera _mainCamera;

    [Header("Agent References")]
    [SerializeField] private AgentGOAP _goapAgent;
    [SerializeField] private WorldStateSensor _worldStateSensor;
    [SerializeField] private ActionScheduler _actionScheduler;

    private void Awake()
    {
        if (_goapAgent == null) _goapAgent = GetComponent<AgentGOAP>();
        if (_worldStateSensor == null) _worldStateSensor = GetComponent<WorldStateSensor>();
        if (_actionScheduler == null) _actionScheduler = GetComponent<ActionScheduler>();
        if (_mainCamera == null) _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        UpdateDebugText();
        FaceCamera();
    }

    private void UpdateDebugText()
    {
        if (_debugText == null || _goapAgent == null || _worldStateSensor == null || _actionScheduler == null)
        {
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("<b>Last Plan Time:</b>");
        sb.AppendLine($"{_goapAgent.name} - Plan: {_goapAgent.LastPlanTimeMs:F2} ms");
        
        sb.AppendLine();
        sb.AppendLine("<b>GOAL Actuel:</b>");

        if (_goapAgent.GoalState.Equals(_goapAgent.offensiveGoal))
        {
            sb.AppendLine("Offensive :");
            sb.AppendLine("- NeedsToRelax, false");
            sb.AppendLine("- IsEnemyDead, true");
        }
        else if (_goapAgent.GoalState.Equals(_goapAgent.defensiveGoal))
        {
            sb.AppendLine("Defensive : ");
            sb.AppendLine("- ShouldFlee, false");
            sb.AppendLine("- IsHurt, false");
            sb.AppendLine("- NeedsToRelax, false");
        }
        else
        {
            sb.AppendLine("Unknow Goal");
        }

        sb.AppendLine();

        sb.AppendLine("<b>Plan Actuel:</b>");
        Queue<GOAPAction> currentPlan = _goapAgent.GetCurrentPlan();
        if (currentPlan != null && currentPlan.Any())
        {
            foreach (var action in currentPlan)
            {
                sb.AppendLine($"- {action.ActionName}");
            }
        }
        else
        {
            sb.AppendLine("- Aucun plan");
        }
        sb.AppendLine();

        sb.AppendLine("<b>Action en cours:</b>");
        if (_actionScheduler.CurrentAction != null)
        {
            sb.AppendLine($"- {_actionScheduler.CurrentAction.ActionName}");
        }
        else
        {
            sb.AppendLine("- Aucune");
        }
        sb.AppendLine();

        sb.AppendLine("<b>World State:</b>");
        WorldStateBit currentState = _worldStateSensor.GetCurrentWorldState(); 
        if (currentState != null)
        {
            foreach (WorldStateKey key in System.Enum.GetValues(typeof(WorldStateKey)))
            {
                bool val = currentState.Get(key);
                string colorTag = val ? "<color=#00FF00>" : "<color=#808080>";
                sb.AppendLine($"{colorTag}{key}: {val}</color>");
            }
        }
        else
        {
            sb.AppendLine("- État du monde vide");
        }

        _debugText.text = sb.ToString();
    }

    private void FaceCamera()
    {
        if (_mainCamera != null && _debugText != null)
        {
            _debugText.transform.parent.LookAt(_debugText.transform.parent.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
        }
    }
}