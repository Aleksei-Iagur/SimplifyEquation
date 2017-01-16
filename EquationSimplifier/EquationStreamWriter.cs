using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace SimplifyEquation
{
    public class EquationStreamWriter
    {
        public void WriteProcessedEquations(StreamWriter streamWriter, ConcurrentDictionary<int, string> processedEquations, EventWaitHandle equationAddedHandle)
        {
            var currentLineNumber = 0;
            using (streamWriter)
            {
                while (true)
                {
                    string processedEquation;
                    while (!processedEquations.TryRemove(currentLineNumber, out processedEquation))
                    {
                        equationAddedHandle.WaitOne();
                    }

                    if (processedEquation == null)
                    {
                        break;
                    }
                    
                    streamWriter.WriteLine(processedEquation);
                    currentLineNumber++;
                }
            }
        }
    }
}
