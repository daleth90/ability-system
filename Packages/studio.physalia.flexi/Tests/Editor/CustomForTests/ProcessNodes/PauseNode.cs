namespace Physalia.Flexi.Tests
{
    [NodeCategoryForTests]
    public class PauseNode : DefaultProcessNode
    {
        protected override AbilityState OnExecute()
        {
            return AbilityState.PAUSE;
        }

        public override bool CanResume(IResumeContext resumeContext)
        {
            return true;
        }

        protected override AbilityState OnResume(IResumeContext resumeContext)
        {
            return AbilityState.RUNNING;
        }
    }
}
