using System.Collections.Generic;

namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class ConditionalModifierNode : DefaultProcessNode
    {
        public Inport<IReadOnlyList<Actor>> actorsPort;
        public Inport<bool> enabledPort;
        public Variable<List<StatModifier>> modifiers;

        protected override AbilityState OnExecute()
        {
            IReadOnlyList<Actor> owners = actorsPort.GetValue();
            if (enabledPort.GetValue())
            {
                for (var i = 0; i < owners.Count; i++)
                {
                    owners[i].AppendModifiers(modifiers.Value);
                }
            }

            return AbilityState.RUNNING;
        }
    }
}
