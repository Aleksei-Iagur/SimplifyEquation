using System.Configuration;

namespace SimplifyEquation
{
    class ConfigurationHelper
    {
        private const string ConsumerCountKey = "consumers-count";
        private const string ProducerConsumerQueueSizeKey = "producer-consumer-queue-size";
        private const string ProcessedEquationsDictionarySizeKey = "processed-equations-dictionary-size";

        public static int ConsumerCount => GetIntegerValueFromConfigFile(ConsumerCountKey);
        public static int ProducerConsumerQueueSize => GetIntegerValueFromConfigFile(ProducerConsumerQueueSizeKey);
        public static int ProcessedEquationsDictionarySize => GetIntegerValueFromConfigFile(ProcessedEquationsDictionarySizeKey);

        private static int GetIntegerValueFromConfigFile(string key)
        {
            int value;
            var configString = ConfigurationManager.AppSettings[key];
            if (!int.TryParse(configString, out value))
            {
                throw new ConfigurationErrorsException($"{key} cannot be parsed from configuration file.");
            }

            return value;
        }
    }
}
