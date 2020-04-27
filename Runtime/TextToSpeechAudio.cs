using System;
using UnityEngine;
using System.Runtime.Serialization;
using Innoactive.Creator.Core;
using Innoactive.Creator.Core.Audio;
using Innoactive.Creator.Core.Attributes;
using Innoactive.Creator.Core.Configuration;
using Innoactive.Creator.Core.Internationalization;

namespace Innoactive.Creator.TextToSpeech.Audio
{
    /// <summary>
    /// This class retrieves and stores AudioClips generated based in a provided localized text. 
    /// </summary>
    [DataContract(IsReference = true)]
    public class TextToSpeechAudio : IAudioData
    {
        private bool isLoading;
        private LocalizedString text;
        
        public string Name { get; } = "TextToSpeech";

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

        /// <summary>
        /// True when there is an Audio Clip loaded.
        /// </summary>
        public bool HasAudioClip
        {
            get
            {
                return AudioClip != null;
            }
        }

        /// <summary>
        /// Returns true only when is busy loading an Audio Clip.
        /// </summary>
        public bool IsLoading
        {
            get { return isLoading; }
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

            isLoading = true;
            
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
            
            isLoading = false;
        }
    }
}
