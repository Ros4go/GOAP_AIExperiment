using System;
using System.Collections;
using System.Linq;
using System.Text;

public enum WorldStateKey
{
    IsEnemyDead,
    HasWeapon,
    IsNearWeapon,
    IsNearEnemy,
    IsHurt,
    ShouldFlee,
    SmokedRecently,
    NeedsToRelax,
    IsDead
}

public class WorldStateBit
{
    private BitArray bits;
    private BitArray _hasValue;

    public WorldStateBit()
    {
        int size = Enum.GetValues(typeof(WorldStateKey)).Length;
        bits = new BitArray(size);
        _hasValue = new BitArray(size);
    }

    public void Set(WorldStateKey key, bool value)
    {
        bits[(int)key] = value;
        _hasValue[(int)key] = true;
    }

    public bool Get(WorldStateKey key)
    {
        return bits[(int)key];
    }

    public bool TryGet(WorldStateKey key, out bool value)
    {
        int index = (int)key;
        value = bits[index];
        return _hasValue[index];
    }

    public bool Matches(WorldStateBit goal)
    {
        for (int i = 0; i < bits.Length; i++)
        {
            if (goal._hasValue[i])
            {
                if (!_hasValue[i] || bits[i] != goal.bits[i])
                    return false;
            }
        }
        return true;
    }

    public WorldStateBit ApplyEffect(WorldStateBit effects)
    {
        WorldStateBit result = Clone();
        for (int i = 0; i < bits.Length; i++)
        {
            if (effects._hasValue[i])
            {
                result.bits[i] = effects.bits[i];
                result._hasValue[i] = true;
            }
        }
        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj is not WorldStateBit other || other.bits.Length != bits.Length)
            return false;

        for (int i = 0; i < bits.Length; i++)
            if (_hasValue[i] != other._hasValue[i] || bits[i] != other.bits[i])
                return false;

        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        for (int i = 0; i < bits.Length; i++)
        {
            hash = hash * 23 + (bits[i] ? 1 : 0);
            hash = hash * 23 + (_hasValue[i] ? 1 : 0);
        }
        return hash;
    }

    public WorldStateBit Clone()
    {
        WorldStateBit clone = new WorldStateBit();
        for (int i = 0; i < bits.Length; i++)
        {
            if (_hasValue[i])
            {
                clone.bits[i] = bits[i];
                clone._hasValue[i] = true;
            }
        }
        return clone;
    }

    public override string ToString()
    {
        var keys = Enum.GetValues(typeof(WorldStateKey)).Cast<WorldStateKey>();
        var builder = new StringBuilder();
        builder.Append("{");
        foreach (var key in keys)
        {
            if (TryGet(key, out bool val))
                builder.AppendFormat("{0}: {1}, ", key, val);
        }
        if (builder.Length > 1)
            builder.Length -= 2;
        builder.Append("}");
        return builder.ToString();
    }
}
