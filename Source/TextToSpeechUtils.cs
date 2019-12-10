using Innoactive.Hub.Helper;

namespace Innoactive.Hub.TextToSpeech
{
    public static class TextToSpeechUtils
    {
        /// <summary>
        /// Returns filename which is uniquly identifies the audio by Backend, Language, Voice and also the text.
        /// </summary>
        public static string GetUniqueTextToSpeechFilename(this TextToSpeechConfig config, string text, string format = "wav")
        {
            string hash = string.Format("{0}_{1}", config.Voice, text);
            return string.Format(@"TTS_{0}_{1}_{2}.{3}", config.Provider, config.Language, HashAlgorithmExtension.GetMd5Hash(hash).Replace("-", ""), format);
        }
    }
}