using System;
using UnityEngine;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// TextToSpeechProvider allows to convert text to AudioClips.
    /// </summary>
    public interface ITextToSpeechProvider
    {
        /// <summary>
        /// Used for setting the config file.
        /// </summary>
        void SetConfig(TextToSpeechConfiguration configuration);

        /// <summary>
        /// Loads the AudioClip file for the given text.
        /// </summary>
        void ConvertTextToSpeech(string text, Action<AudioClip> OnFinished);
    }
}