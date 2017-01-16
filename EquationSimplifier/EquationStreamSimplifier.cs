using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimplifyEquation
{
    public class EquationStreamSimplifier
    {
        private readonly AutoResetEvent _equationAddedHandler;
        private readonly ProducerConsumerQueue _simplificationJobs;
        private readonly EquationStreamReader _equationStreamReader;
        private readonly EquationStreamWriter _equationStreamWriter;
        private readonly ConcurrentDictionary<int, string> _processedEquations;

        public EquationStreamSimplifier(int producerConsumerQueueSize, int consumerCount, int processedEquationsDictionarySize)
        {
            _equationAddedHandler = new AutoResetEvent(false);
            _equationStreamReader = new EquationStreamReader();
            _equationStreamWriter = new EquationStreamWriter();
            _simplificationJobs = new ProducerConsumerQueue(producerConsumerQueueSize, consumerCount);
            _processedEquations = new ConcurrentDictionary<int, string>(consumerCount, processedEquationsDictionarySize);
        }

        public void SimplifyAndSaveEquations(StreamReader streamReader, StreamWriter streamWriter, IEquationSimplifier simplifier, CancellationToken token)
        {
            var reader = Task.Factory.StartNew(() =>
            {
                _equationStreamReader.QueueEquationsFromFileForProcessing(streamReader, simplifier, _processedEquations, _simplificationJobs, _equationAddedHandler, token);
                _simplificationJobs.Dispose();
            }, 
            token);
            
            var writer = Task.Factory.StartNew(() => _equationStreamWriter.WriteProcessedEquations(streamWriter, _processedEquations, _equationAddedHandler), token);
            Task.WaitAll(reader, writer);
        }
    }
}
