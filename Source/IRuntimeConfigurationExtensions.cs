using Innoactive.Hub.Training.Configuration;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// TextToSpeech extensions methods for <see cref="IRuntimeConfiguration"/>.
    /// </summary>
    public static class IRuntimeConfigurationExtensions
    {
        /// <summary>
        /// Text to speech configuration.
        /// </summary>
        private static TextToSpeechConfiguration textToSpeechConfiguration;
        
        /// <summary>
        /// Return loaded <see cref="TextToSpeechConfiguration"/>.
        /// </summary>
        public static TextToSpeechConfiguration GetTextToSpeechConfiguration(this IRuntimeConfiguration runtimeConfiguration)
        {
            if (textToSpeechConfiguration == null)
            {
                textToSpeechConfiguration = TextToSpeechConfiguration.LoadConfiguration();
            }
            
            return textToSpeechConfiguration;
        }
        
        /// <summary>
        /// Loads a new <see cref="TextToSpeechConfiguration"/>
        /// </summary>
        public static void SetTextToSpeechConfiguration(this IRuntimeConfiguration runtimeConfiguration, TextToSpeechConfiguration ttsConfiguration)
        {
            textToSpeechConfiguration = ttsConfiguration;
        }
    }
}
