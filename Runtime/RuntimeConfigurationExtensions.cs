using System;
using Innoactive.Creator.Core.Configuration;

namespace Innoactive.Creator.TextToSpeech
{
    /// <summary>
    /// TextToSpeech extensions methods for <see cref="IRuntimeConfiguration"/>.
    /// </summary>
    public static class RuntimeConfigurationExtensions
    {
        /// <summary>
        /// Text to speech configuration.
        /// </summary>
        private static TextToSpeechConfiguration textToSpeechConfiguration;
        
        /// <summary>
        /// Return loaded <see cref="TextToSpeechConfiguration"/>.
        /// </summary>
        public static TextToSpeechConfiguration GetTextToSpeechConfiguration(this BaseRuntimeConfiguration runtimeConfiguration)
        {
            if (textToSpeechConfiguration == null)
            {
                textToSpeechConfiguration = TextToSpeechConfiguration.LoadConfiguration();
            }
            
            return textToSpeechConfiguration;
        }
        
        /// <summary>
        /// Return loaded <see cref="TextToSpeechConfiguration"/>.
        /// </summary>
        [Obsolete("To be more flexible with the Creator development we switched to an abstract class as configuration base, consider using BaseRuntimeConfiguration.")]
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
        public static void SetTextToSpeechConfiguration(this BaseRuntimeConfiguration runtimeConfiguration, TextToSpeechConfiguration ttsConfiguration)
        {
            textToSpeechConfiguration = ttsConfiguration;
        }
        
        /// <summary>
        /// Loads a new <see cref="TextToSpeechConfiguration"/>
        /// </summary>
        [Obsolete("To be more flexible with the Creator development we switched to an abstract class as configuration base, consider using BaseRuntimeConfiguration.")]
        public static void SetTextToSpeechConfiguration(this IRuntimeConfiguration runtimeConfiguration, TextToSpeechConfiguration ttsConfiguration)
        {
            textToSpeechConfiguration = ttsConfiguration;
        }
    }
}
