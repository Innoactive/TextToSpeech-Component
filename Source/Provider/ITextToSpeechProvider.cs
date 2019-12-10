using Innoactive.Hub.SDK;
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
        void SetConfig(TextToSpeechConfig config);

        /// <summary>
        /// Loads the AudioClip file for the given text async.
        /// </summary>
        IAsyncTask<AudioClip> ConvertTextToSpeech(string text);
    }
}