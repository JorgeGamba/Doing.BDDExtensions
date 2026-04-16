using System;
using System.Threading.Tasks;

namespace Doing.BDDExtensions
{
    /// <summary>
    /// Captures exceptions for assertion instead of letting them fail the test.
    /// </summary>
    public static class Catch
    {
        /// <summary>
        /// Executes <paramref name="action"/> and returns the thrown exception,
        /// or <c>null</c> if no exception occurred.
        /// </summary>
        public static Exception Exception(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        /// <summary>
        /// Captures an exception from an async operation. Unwraps <see cref="AggregateException"/>
        /// when it contains a single inner exception, surfacing the original exception directly.
        /// </summary>
        public static Exception Exception(Func<Task> action)
        {
            try
            {
                action().GetAwaiter().GetResult();
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1)
            {
                return ex.InnerExceptions[0];
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }
    }
}
