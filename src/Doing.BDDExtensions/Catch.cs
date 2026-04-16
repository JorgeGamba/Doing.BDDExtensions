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
        /// Executes <paramref name="action"/> and returns the thrown exception already cast
        /// to <typeparamref name="TException"/>, or <c>null</c> if no exception occurred
        /// or the exception is not of the expected type.
        /// </summary>
        public static TException Exception<TException>(Action action) where TException : Exception =>
            Exception(action) as TException;

        /// <summary>
        /// Captures an exception from an async operation, or <c>null</c> if no exception occurred.
        /// </summary>
        public static Exception Exception(Func<Task> action)
        {
            try
            {
                action().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }

        /// <summary>
        /// Captures an exception from an async operation, already cast to <typeparamref name="TException"/>.
        /// Returns <c>null</c> if no exception occurred or the exception is not of the expected type.
        /// </summary>
        public static TException Exception<TException>(Func<Task> action) where TException : Exception =>
            Exception(action) as TException;
    }
}
