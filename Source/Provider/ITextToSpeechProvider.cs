using UnityEngine;
using Innoactive.Hub.SDK;

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
        /// Loads the AudioClip file for the given text async.
        /// </summary>
        IAsyncTask<AudioClip> ConvertTextToSpeech(string text);
    }
}