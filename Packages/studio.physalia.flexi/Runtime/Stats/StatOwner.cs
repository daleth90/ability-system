using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Physalia.Flexi
{
    /// <summary>
    /// StatOwner is a container of stats and modifiers.
    /// </summary>
    public class StatOwner
    {
        private readonly Dictionary<int, Stat> stats = new();
        private readonly List<StatModifier> modifiers = new();

        public IReadOnlyDictionary<int, Stat> Stats => stats;
        public IReadOnlyList<StatModifier> Modifiers => modifiers;

        public StatOwner()
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddStat<TEnum>(TEnum statId, int baseValue) where TEnum : Enum
        {
            AddStat(CastTo<int>.From(statId), baseValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveStat<TEnum>(TEnum statId) where TEnum : Enum
        {
            RemoveStat(CastTo<int>.From(statId));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Stat GetStat<TEnum>(TEnum statId) where TEnum : Enum
        {
            return GetStat(CastTo<int>.From(statId));
        }

        public void AddStat(int statId, int baseValue)
        {
            if (stats.ContainsKey(statId))
            {
                Logger.Error($"Add stat failed! Owner already has stat: {statId}");
                return;
            }

            var stat = new Stat(statId, baseValue);
            stats.Add(statId, stat);
        }

        public void RemoveStat(int statId)
        {
            stats.Remove(statId);
        }

        public Stat GetStat(int statId)
        {
            if (!stats.TryGetValue(statId, out Stat stat))
            {
                return null;
            }

            return stat;
        }

        public void AppendModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void AppendModifiers(IReadOnlyList<StatModifier> modifiers)
        {
            this.modifiers.AddRange(modifiers);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            modifiers.Remove(modifier);
        }

        public void ClearAllModifiers()
        {
            modifiers.Clear();
        }

        internal void ResetAllStats()
        {
            foreach (Stat stat in stats.Values)
            {
                stat.CurrentValue = stat.CurrentBase;
            }
        }
    }
}
