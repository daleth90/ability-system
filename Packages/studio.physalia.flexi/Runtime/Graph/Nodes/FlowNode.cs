using System.Collections.Generic;

namespace Physalia.Flexi
{
    public abstract class FlowNode : Node
    {
        public abstract FlowNode Next { get; }

        /// <summary>
        /// This property only works when the FlowRunner is in EventTriggerMode.EACH_NODE.
        /// </summary>
        public virtual bool ShouldTriggerChainEvents => true;

        // Note: FlowNode shouldn't be inherited outside of this assembly
        internal FlowNode()
        {

        }

        internal AbilityState Execute()
        {
            EvaluateInports();
            return DoLogic();
        }

        private protected void EvaluateInports()
        {
            IReadOnlyList<Inport> inports = Inports;
            for (var i = 0; i < inports.Count; i++)
            {
                Inport inport = inports[i];
                IReadOnlyList<Port> connections = inport.GetConnections();
                for (var j = 0; j < connections.Count; j++)
                {
                    var outport = connections[j] as Outport;
                    if (outport.Node is ValueNode valueNode)
                    {
                        valueNode.Evaluate();
                    }
                }
            }
        }

        private protected abstract AbilityState DoLogic();

        public virtual bool CanResume(IResumeContext resumeContext)
        {
            return false;
        }

        internal AbilityState Resume(IResumeContext resumeContext)
        {
            return OnResume(resumeContext);
        }

        protected virtual AbilityState OnResume(IResumeContext resumeContext)
        {
            return AbilityState.RUNNING;
        }

        protected internal virtual AbilityState Tick()
        {
            return AbilityState.PAUSE;
        }

        protected void PushSelf()
        {
            Flow?.Push(this);
        }

        protected void EnqueueEvent(IEventContext eventContext)
        {
            Flow.Core?.EnqueueEvent(eventContext);
        }
    }
}
