using System;
using System.Linq;
using System.Reflection;

namespace Doing.BDDExtensions
{
    /// <summary>
    /// Base class for BDD specifications. Override <see cref="Given"/> to set up preconditions
    /// and <see cref="When"/> to perform the action being specified. Both support <c>async void</c>.
    /// </summary>
    public abstract class FeatureSpecifications
    {
        /// <inheritdoc cref="FeatureSpecifications"/>
        protected FeatureSpecifications()
        {
            ThrowSteps();
        }

        /// <summary>
        /// Override to set up preconditions for the specification. Executed parent-to-child
        /// through the inheritance hierarchy before <see cref="When"/> runs.
        /// </summary>
        public virtual void Given()
        {
        }

        /// <summary>
        /// Override to perform the single action being specified. Executed once,
        /// after all <see cref="Given"/> methods in the hierarchy have completed.
        /// </summary>
        public virtual void When()
        {
        }


        private void ThrowSteps()
        {
            InvokeBaseGivenIfExists(this.GetType());
            AsyncRunner.Run(When);
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
                AsyncRunner.Run(action);
            }
        }
    }
}