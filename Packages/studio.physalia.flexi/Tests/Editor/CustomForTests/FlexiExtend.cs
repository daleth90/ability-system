namespace Physalia.Flexi.Tests
{
    public class AbilityContainer : AbilityDataContainer
    {
        public Actor actor;
    }

    public abstract class EntryNode<TEventContext> : EntryNode<AbilityContainer, TEventContext> where TEventContext : IEventContext
    {
        public Actor Actor => Container.actor;
    }

    public abstract class ProcessNode : ProcessNode<AbilityContainer>
    {
        public Actor Actor => Container.actor;
    }

    public abstract class ValueNode : ValueNode<AbilityContainer>
    {
        public Actor Actor => Container.actor;
    }
}
