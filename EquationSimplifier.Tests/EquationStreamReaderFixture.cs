using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace SimplifyEquation.Tests
{
    [TestFixture]
    class EquationStreamReaderFixture
    {
        [Test]
        public void Queue_ThreeEntries_EnqueuesThreeRealElementsAndOneTerminal()
        {
            var sut = new EquationStreamReader();

            var streamReader = GetStreamReader();
            var dic = new ConcurrentDictionary<int, string>();
            var equationAddedHandler = new Mock<EventWaitHandle>(false, EventResetMode.AutoReset);
            int queueSize = 3;
            int consumerCount = 5;
            var queue = new Mock<ProducerConsumerQueue>(queueSize, consumerCount);
            var simplifier = new Mock<IEquationSimplifier>();
            sut.QueueEquationsFromFileForProcessing(streamReader, simplifier.Object, dic, queue.Object, equationAddedHandler.Object, CancellationToken.None);

            Assert.AreEqual(4, dic.Count);
        }

        private StreamReader GetStreamReader()
        {
            var sb = new StringBuilder();
            sb.AppendLine("x^2 = 9");
            sb.AppendLine("13 = 13");
            sb.AppendLine("-(4.44y - 10 + 5z^2) = -15x^7");
            byte[] bytes = Encoding.ASCII.GetBytes(sb.ToString());
            var stream = new MemoryStream(bytes);
            return new StreamReader(stream);
        }
    }
}
