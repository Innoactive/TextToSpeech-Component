using System.Runtime.Serialization;
using Innoactive.Hub.TextToSpeech;
using Innoactive.Hub.Training.Attributes;
using Innoactive.Hub.Training.Configuration;
using Newtonsoft.Json;
using UnityEngine;

namespace Innoactive.Hub.Training.Audio
{
    [DataContract(IsReference = true)]
    public class TextToSpeechAudio : IAudioData
    {
        private static readonly Common.Logging.ILog logger = Logging.LogManager.GetLogger<TextToSpeechAudio>();

        private LocalizedString text;

        [DataMember]
        [UsesSpecificTrainingDrawer("TextToSpeechAudioDataLocalizedStringDrawer")]
        public LocalizedString Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                InitializeAudioClip();
            }
        }

        [JsonConstructor]
        protected TextToSpeechAudio()
        {
            text = new LocalizedString();
        }

        public TextToSpeechAudio(LocalizedString text)
        {
            Text = text;
        }

        public bool HasAudioClip
        {
            get
            {
                return AudioClip != null;
            }
        }

        public AudioClip AudioClip { get; private set; }

        private void InitializeAudioClip()
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            if (Text == null || string.IsNullOrEmpty(Text.Value))
            {
                logger.Warn("No text provided.");
                return;
            }

            try
            {
                ITextToSpeechProvider provider;
                if (RuntimeConfigurator.Configuration.TextToSpeechConfig == null)
                {
                    provider = TextToSpeechProviderFactory.Instance.CreateProvider();
                }
                else
                {
                    provider = TextToSpeechProviderFactory.Instance.CreateProvider(RuntimeConfigurator.Configuration.TextToSpeechConfig);
                }

                provider.ConvertTextToSpeech(Text.Value)
                    .OnFinished((clip) => { AudioClip = clip; })
                    .Execute();
            }
            catch (TextToSpeechProviderFactory.NoConfigurationFoundException)
            {
                logger.Warn("No text to speech configuration found!");
            }
        }
    }
}
