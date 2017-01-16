using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimplifyEquation
{
    public class EquationStreamReader
    {
        public void QueueEquationsFromFileForProcessing(StreamReader streamReader, IEquationSimplifier simplifier, ConcurrentDictionary<int, string> processedEquations, ProducerConsumerQueue simplificationJobs, EventWaitHandle equationAddedHandler, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();
            int currentRow = 0;

            using (streamReader)
            {
                while (!streamReader.EndOfStream)
                {
                    var equation = streamReader.ReadLine();

                    var action = GetAction(simplifier, equation, processedEquations, currentRow, equationAddedHandler);
                    var task = simplificationJobs.Enqueue(action, cancellationToken);

                    tasks.Add(task);
                    currentRow++;
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            
            Task.WaitAll(tasks.ToArray());

            processedEquations.TryAdd(currentRow, null);
        }

        private Action GetAction(IEquationSimplifier simplifier, string equation, ConcurrentDictionary<int, string> processedEquations, int rowNumber, EventWaitHandle equationAddedHandler)
        {
            return () =>
            {
                string simplifiedEquation;
                try
                {
                    simplifiedEquation = simplifier.Simplify(equation);
                }
                catch (Exception ex)
                {
                    simplifiedEquation = $"[{equation}] was not processed correctly: {ex.Message}";
                }

                var success = processedEquations.TryAdd(rowNumber, simplifiedEquation);

                if (!success)
                {
                    throw new Exception($"Unable to write simplified equation [{simplifiedEquation}] to dictionary.");
                }

                equationAddedHandler.Set();
            };
        }
    }
}
