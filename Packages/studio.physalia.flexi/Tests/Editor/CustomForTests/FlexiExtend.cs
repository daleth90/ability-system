namespace Physalia.Flexi.Tests
{
    public class DefaultAbilityContainer : AbilityContainer
    {
        public CustomFlexiCoreWrapper CoreWrapper;
        public Actor Actor;
    }

    public abstract class DefaultEntryNode : EntryNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultEntryNode<TEventContext> : EntryNode<DefaultAbilityContainer, TEventContext>
        where TEventContext : IEventContext
    {

    }

    public abstract class DefaultProcessNode : ProcessNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultFlowNode : FlowNode<DefaultAbilityContainer>
    {

    }

    public abstract class DefaultValueNode : ValueNode<DefaultAbilityContainer>
    {

    }
}
