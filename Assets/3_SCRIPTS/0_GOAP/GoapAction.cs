using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GOAPKeyValue
{
    public WorldStateKey Key;
    public bool Value;
}

public abstract class GOAPAction : ScriptableObject
{
    public string ActionName = "UnnamedAction";
    public float Cost = 1f;

    [SerializeField]
    public List<GOAPKeyValue> preconditions;

    [SerializeField]
    public List<GOAPKeyValue> effects;

    public WorldStateBit PreconditionsBit
    {
        get
        {
            var bitState = new WorldStateBit();
            foreach (var pre in preconditions)
            {
                bitState.Set(pre.Key, pre.Value);
            }
            return bitState;
        }
    }

    public WorldStateBit ApplyEffects(WorldStateBit from)
    {
        WorldStateBit result = new WorldStateBit();
        foreach (WorldStateKey key in Enum.GetValues(typeof(WorldStateKey)))
            result.Set(key, from.Get(key));

        foreach (var eff in effects)
        {
            result.Set(eff.Key, eff.Value);
        }

        return result;
    }

    public virtual void OnEnable()
    {
        if (preconditions == null) preconditions = new List<GOAPKeyValue>();
        if (effects == null) effects = new List<GOAPKeyValue>();
    }

    public virtual bool IsAchievableInBitState(WorldStateBit state)
    {
        foreach (var precond in preconditions)
        {
            if (!state.TryGet(precond.Key, out bool val) || val != precond.Value)
                return false;
        }
        return true;
    }

    public WorldStateBit EffectsBit
    {
        get
        {
            var bitState = new WorldStateBit();
            foreach (var eff in effects)
            {
                bitState.Set(eff.Key, eff.Value);
            }
            return bitState;
        }
    }

    public abstract bool IsDone(AgentGOAP agent);

    public abstract void PerformAction(AgentGOAP agent);

    public abstract void AbortAction(AgentGOAP agent);
}
