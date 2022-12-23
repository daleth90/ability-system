using System;
using System.Collections.Generic;

namespace Physalia.AbilityFramework
{
    public class AbilitySystem
    {
        private static readonly StatRefreshEvent STAT_REFRESH_EVENT = new();

        public event Action<IEventContext> EventOccurred;
        public event Action<IChoiceContext> ChoiceOccurred;

        public Action<IEventContext> EventResolveMethod;

        private readonly StatOwnerRepository ownerRepository;
        private readonly AbilityRunner runner;
        private readonly AbilityEventQueue eventQueue = new();

        private readonly MacroLibrary macroLibrary = new();

        internal AbilitySystem(StatDefinitionListAsset statDefinitionListAsset, AbilityRunner runner)
        {
            ownerRepository = StatOwnerRepository.Create(statDefinitionListAsset);
            this.runner = runner;
        }

        internal StatOwner CreateOwner()
        {
            return ownerRepository.CreateOwner();
        }

        internal void RemoveOwner(StatOwner owner)
        {
            ownerRepository.RemoveOwner(owner);
        }

        internal StatOwner GetOwner(int id)
        {
            return ownerRepository.GetOwner(id);
        }

        public void LoadMacroGraph(string key, MacroGraphAsset macroGraphAsset)
        {
            macroLibrary.Add(key, macroGraphAsset.Text);
        }

        internal AbilityGraph GetMacroGraph(string key)
        {
            bool success = macroLibrary.TryGetValue(key, out string macroJson);
            if (!success)
            {
                Logger.Error($"[{nameof(AbilitySystem)}] Get macro failed! key: {key}");
                return null;
            }

            AbilityGraph graph = AbilityGraphUtility.Deserialize("", macroJson, macroLibrary);
            return graph;
        }

#if UNITY_5_3_OR_NEWER
        public Ability InstantiateAbility(AbilityAsset abilityAsset)
        {
            return InstantiateAbility(abilityAsset.Data);
        }
#endif

        public Ability InstantiateAbility(AbilityData abilityData)
        {
            var ability = new Ability(this, abilityData);
            ability.Initialize();
            return ability;
        }

        internal AbilityFlow InstantiateAbilityFlow(Ability ability, int index)
        {
            string graphJson = ability.Data.graphJsons[index];
            AbilityGraph graph = AbilityGraphUtility.Deserialize("", graphJson, macroLibrary);
            AbilityFlow flow = new AbilityFlow(this, graph, ability);
            return flow;
        }

        internal void EnqueueEvent(IEventContext eventContext)
        {
            eventQueue.Enqueue(eventContext);
            EventOccurred?.Invoke(eventContext);
        }

        internal void TriggerCachedEvents()
        {
            if (eventQueue.Count == 0)
            {
                return;
            }

            runner.PushNewAbilityQueue();
            while (eventQueue.Count > 0)
            {
                IEventContext eventContext = eventQueue.Dequeue();
                if (EventResolveMethod != null)
                {
                    EventResolveMethod.Invoke(eventContext);
                }
                else
                {
                    EnqueueAbilitiesForAllOwners(eventContext);
                }
            }

            runner.PopEmptyQueues();
        }

        private void EnqueueAbilitiesForAllOwners(IEventContext eventContext)
        {
            foreach (StatOwner owner in ownerRepository.Owners)
            {
                _ = TryEnqueueAbilities(owner.Abilities, eventContext);
            }
        }

        public bool TryEnqueueAndRunAbility(Ability ability, IEventContext eventContext)
        {
            bool success = TryEnqueueAbility(ability, eventContext);
            if (success)
            {
                Run();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryEnqueueAndRunAbilities(IReadOnlyList<Ability> abilities, IEventContext eventContext)
        {
            bool success = TryEnqueueAbilities(abilities, eventContext);
            if (success)
            {
                Run();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryEnqueueAbility(Ability ability, IEventContext eventContext)
        {
            bool hasAnyEnqueued = false;
            for (var i = 0; i < ability.Flows.Count; i++)
            {
                AbilityFlow abilityFlow = ability.Flows[i];
                if (abilityFlow.CanExecute(eventContext))
                {
                    hasAnyEnqueued = true;
                    EnqueueAbilityFlow(abilityFlow, eventContext);
                    break;
                }
            }

            return hasAnyEnqueued;
        }

        public bool TryEnqueueAbilities(IReadOnlyList<Ability> abilities, IEventContext eventContext)
        {
            bool hasAnyEnqueued = false;
            for (var i = 0; i < abilities.Count; i++)
            {
                Ability ability = abilities[i];
                for (var j = 0; j < ability.Flows.Count; j++)
                {
                    AbilityFlow abilityFlow = ability.Flows[j];
                    if (abilityFlow.CanExecute(eventContext))
                    {
                        // Move to next ability
                        hasAnyEnqueued = true;
                        EnqueueAbilityFlow(abilityFlow, eventContext);
                        break;
                    }
                }
            }

            return hasAnyEnqueued;
        }

        private void EnqueueAbilityFlow(AbilityFlow flow, IEventContext eventContext)
        {
            flow.Reset();
            flow.SetPayload(eventContext);
            runner.Add(flow);
        }

        public void Run()
        {
            runner.Run(this);
        }

        public void Resume(IResumeContext resumeContext)
        {
            runner.Resume(this, resumeContext);
        }

        public void RefreshStatsAndModifiers()
        {
            ownerRepository.RefreshStatsForAllOwners();
            DoStatRefreshLogicForAllOwners();
            ownerRepository.RefreshStatsForAllOwners();
        }

        /// <remarks>
        /// StatRefresh does not run with other events and abilities. It runs in another line.
        /// </remarks>
        private void DoStatRefreshLogicForAllOwners()
        {
            foreach (StatOwner owner in ownerRepository.Owners)
            {
                for (var i = 0; i < owner.AbilityFlows.Count; i++)
                {
                    AbilityFlow abilityFlow = owner.AbilityFlows[i];
                    if (abilityFlow.CanExecute(STAT_REFRESH_EVENT))
                    {
                        abilityFlow.Reset();
                        abilityFlow.SetPayload(STAT_REFRESH_EVENT);
                        abilityFlow.Execute();
                    }
                }
            }
        }

        internal void TriggerChoice(IChoiceContext context)
        {
            ChoiceOccurred?.Invoke(context);
        }
    }
}
