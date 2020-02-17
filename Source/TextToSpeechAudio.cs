using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Innoactive.Hub.TextToSpeech;
using Innoactive.Hub.Training.Attributes;
using Innoactive.Hub.Training.Configuration;

namespace Innoactive.Hub.Training.Audio
{
    [DataContract(IsReference = true)]
    public class TextToSpeechAudio : IAudioData
    {
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
                Debug.LogWarning("No text provided.");
                return;
            }

            try
            {
                TextToSpeechConfiguration ttsConfiguration = RuntimeConfigurator.Configuration.GetTextToSpeechConfiguration();
                ITextToSpeechProvider provider = TextToSpeechProviderFactory.Instance.CreateProvider(ttsConfiguration);

                provider.ConvertTextToSpeech(Text.Value, clip => AudioClip = clip);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(exception.Message);
            }
        }
    }
}
