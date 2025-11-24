using System.Collections.Generic;
using System.Linq;
using UnityEngine; 
using System.Diagnostics;

public class GOAPPlanner
{
    private class Node
    {
        public Node Parent; 
        public float RunningCost;
        public WorldStateBit State;
        public GOAPAction Action; 

        public Node(Node parent, float runningCost, WorldStateBit state, GOAPAction action)
        {
            Parent = parent;
            RunningCost = runningCost;
            State = state;
            Action = action;
        }
    }

    public Queue<GOAPAction> Plan(AgentGOAP agent, WorldStateBit currentWorldState, WorldStateBit goal, HashSet<GOAPAction> availableActions)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        List<Node> leaves = new List<Node>();
        List<Node> openSet = new List<Node>();
        openSet.Add(new Node(null, 0f, currentWorldState, null));
        Dictionary<WorldStateBit, float> closedSet = new();

        while (openSet.Any())
        {
            Node currentNode = openSet.OrderBy(n => n.RunningCost).First();
            openSet.Remove(currentNode);

            if (closedSet.TryGetValue(currentNode.State, out float existingCost) && existingCost <= currentNode.RunningCost)
                continue;

            closedSet[currentNode.State] = currentNode.RunningCost;

            if (currentNode.State.Matches(goal))
            {
                leaves.Add(currentNode);
                continue;
            }

            foreach (GOAPAction action in availableActions.OrderBy(a => Random.value))
            {
                if (action.IsAchievableInBitState(currentNode.State))
                {
                    WorldStateBit newState = currentNode.State.ApplyEffect(action.EffectsBit);
                    float newCost = currentNode.RunningCost + action.Cost;
                    Node newNode = new Node(currentNode, newCost, newState, action);
                    openSet.Add(newNode);
                }
            }
        }

        sw.Stop();

        if (!leaves.Any())
        {
            UnityEngine.Debug.LogWarning($"GOAP: Aucun plan trouvé. Temps: {sw.Elapsed.TotalMilliseconds:F3} ms");
            return null;
        }

        Node cheapest = leaves.OrderBy(n => n.RunningCost).First();

        Queue<GOAPAction> plan = new();
        while (cheapest != null && cheapest.Action != null)
        {
            plan.Enqueue(cheapest.Action);
            cheapest = cheapest.Parent;
        }

        UnityEngine.Debug.Log($"GOAPPlanner: Plan généré en {sw.Elapsed.TotalMilliseconds:F3} ms");
 
        if (Application.isPlaying)
        {
            agent.LastPlanTimeMs = (float)sw.Elapsed.TotalMilliseconds;
        }

        return new Queue<GOAPAction>(plan.Reverse());
    }

}