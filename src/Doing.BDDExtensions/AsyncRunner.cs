using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Doing.BDDExtensions
{
    /// <summary>
    /// Executes an action that may be <c>async void</c>, blocking until all async
    /// continuations complete. Synchronous actions pass through with minimal overhead.
    /// </summary>
    internal static class AsyncRunner
    {
        public static void Run(Action action)
        {
            var previousContext = SynchronizationContext.Current;
            var trackingContext = new TrackingContext();
            SynchronizationContext.SetSynchronizationContext(trackingContext);
            try
            {
                action();
                trackingContext.WaitForPendingOperations();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(previousContext);
                trackingContext.Dispose();
            }
        }

        private class TrackingContext : SynchronizationContext, IDisposable
        {
            private int _pendingOperations;
            private readonly ManualResetEventSlim _completed = new ManualResetEventSlim(true);
            private ExceptionDispatchInfo _capturedException;

            public override void OperationStarted()
            {
                Interlocked.Increment(ref _pendingOperations);
                _completed.Reset();
            }

            public override void OperationCompleted()
            {
                if (Interlocked.Decrement(ref _pendingOperations) == 0)
                    _completed.Set();
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                OperationStarted();
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        d(state);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.CompareExchange(
                            ref _capturedException, ExceptionDispatchInfo.Capture(ex), null);
                    }
                    finally
                    {
                        OperationCompleted();
                    }
                }, null);
            }

            public override void Send(SendOrPostCallback d, object state) =>
                d(state);

            public void WaitForPendingOperations()
            {
                _completed.Wait();
                _capturedException?.Throw();
            }

            public void Dispose() =>
                _completed.Dispose();
        }
    }
}
