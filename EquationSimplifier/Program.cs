using System;
using System.IO;
using System.Threading;

namespace SimplifyEquation
{
    class Program
    {
        private static CancellationTokenSource _cancellationTokenSource;

        static void Main(string[] args)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += ExitHandler;
            
            if (args.Length == 0)
            {
                StartInteractiveMode();
                return;
            }

            WorkWithFiles(args);
        }

        private static void WorkWithFiles(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine($"Wrong number of arguments: {args.Length}.");
                return;
            }

            var filename = args[0];

            var streamSimplifier = GetStreamGetWithConfigParameters();
            var reader = GetStreamReader(filename);
            var writer = GetStreamWriter(filename);
            var equationSimplifier = new EquationSimplifier();

            if (reader == null || writer == null)
            {
                return;
            }

            Console.WriteLine("Processing started...");

            streamSimplifier.SimplifyAndSaveEquations(reader, writer, equationSimplifier, _cancellationTokenSource.Token);

            Console.WriteLine("Processing is over. Press <ENTER> to exit.");
            Console.ReadLine();
        }

        private static StreamWriter GetStreamWriter(string filename)
        {
            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(filename + ".out", false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured while opening the file {filename} for writing: {ex.Message}");
            }

            return writer;
        }

        private static StreamReader GetStreamReader(string filename)
        {
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured while opening the file {filename}: {ex.Message}");
            }

            return reader;
        }

        private static EquationStreamSimplifier GetStreamGetWithConfigParameters()
        {
            return new EquationStreamSimplifier( ConfigurationHelper.ProducerConsumerQueueSize, 
                                               ConfigurationHelper.ConsumerCount, 
                                               ConfigurationHelper.ProcessedEquationsDictionarySize);
        }

        private static void ExitHandler(object sender, ConsoleCancelEventArgs args)
        {
            _cancellationTokenSource.Cancel(true);
        }

        private static void StartInteractiveMode()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter the equation you want to simplify (e.g. \"x^2 + 3.5xy + y = y^2 - xy + y\"):");
                var equation = Console.ReadLine();

                var simplifier = new EquationSimplifier();
                string result;
                try
                {
                    result = simplifier.Simplify(equation);
                }
                catch (Exception ex)
                {
                    result = $"An error has occured: {ex.Message}";
                }

                Console.WriteLine(result);
            }
        }
    }
}