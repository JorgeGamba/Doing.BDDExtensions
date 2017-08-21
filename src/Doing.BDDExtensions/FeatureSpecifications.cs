using System;
using System.Linq;
using System.Reflection;

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
            InvokeBaseGivenIfExists(this.GetType());
            When();
        }

        // TODO: Definitely improvable, it should be a some way of all or nothing, to kill this method or to implement a convention over configuarion solution
        private void InvokeBaseGivenIfExists(Type type)
        {
            if (type == null || type == typeof(FeatureSpecifications)) return;
            InvokeBaseGivenIfExists(type.BaseType);

            var method = type.GetMethods().FirstOrDefault(x => x.Name == "Given" && x.DeclaringType == type);
            if (method != null)
            {
                var ftn = method.MethodHandle.GetFunctionPointer();
                var action = (Action)Activator.CreateInstance(typeof(Action), this, ftn);
                action();
            }
        }
    }
}