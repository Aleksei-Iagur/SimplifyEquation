using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SimplifyEquation
{
    public class ProducerConsumerQueue : IDisposable
    {
        readonly BlockingCollection<WorkItem> _queue;

        public ProducerConsumerQueue(int queueSize, int consumerCount)
        {
            _queue = new BlockingCollection<WorkItem>(queueSize);

            for (int i = 0; i < consumerCount; i++)
            {
                Task.Factory.StartNew(Consume);
            }
        }

        public Task Enqueue(Action action, CancellationToken? cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>();
            _queue.Add(new WorkItem(tcs, action, cancellationToken));
            return tcs.Task;
        }

        public void Dispose()
        {
            _queue.CompleteAdding();
        }

        private void Consume()
        {
            foreach (WorkItem workItem in _queue.GetConsumingEnumerable())
            {
                if (workItem.CancellationToken.HasValue && workItem.CancellationToken.Value.IsCancellationRequested)
                {
                    workItem.TaskSource.SetCanceled();
                }
                else
                {
                    try
                    {
                        workItem.Action();
                        workItem.TaskSource.SetResult(null);
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken == workItem.CancellationToken)
                        {
                            workItem.TaskSource.SetCanceled();
                        }
                        else
                        {
                            workItem.TaskSource.SetException(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        workItem.TaskSource.SetException(ex);
                    }
                }
            }
        }

        class WorkItem
        {
            public readonly TaskCompletionSource<object> TaskSource;
            public readonly Action Action;
            public readonly CancellationToken? CancellationToken;

            public WorkItem(TaskCompletionSource<object> taskSource, Action action, CancellationToken? cancellationToken)
            {
                TaskSource = taskSource;
                Action = action;
                CancellationToken = cancellationToken;
            }
        }
    }
}
