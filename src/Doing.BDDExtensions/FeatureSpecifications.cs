namespace Doing.BDDExtensions
{
    public abstract class FeatureSpecifications
    {
        protected FeatureSpecifications()
        {
            ThrowSteps();
        }

        public virtual void Given()
        {
        }

        public virtual void When()
        {
        }


        private void ThrowSteps()
        {
            Given();
            When();
        }
    }
}