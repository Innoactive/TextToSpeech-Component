using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Innoactive.Hub.TextToSpeech;
using Innoactive.Hub.Training.Attributes;
using Innoactive.Hub.Training.Configuration;
using Innoactive.Creator.Internationalization;

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

        private async void InitializeAudioClip()
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

                AudioClip = await provider.ConvertTextToSpeech(Text.Value);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(exception.Message);
            }
        }
    }
}
