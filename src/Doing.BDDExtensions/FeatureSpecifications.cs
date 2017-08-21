using System;

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
            InvokeBaseGivenIfExists();
            Given();
            When();
        }

        // TODO: Definitely improvable, it should be a some way of all or nothing, to kill this method or to implement a convention over configuarion solution
        private void InvokeBaseGivenIfExists()
        {
            var baseType = GetType().BaseType;
            if (baseType == typeof(FeatureSpecifications)) return;

            var method = baseType?.GetMethod("Given");
            if (method != null)
            {
                var ftn = method.MethodHandle.GetFunctionPointer();
                var action = (Action)Activator.CreateInstance(typeof(Action), this, ftn);
                action();
            }
        }
    }
}