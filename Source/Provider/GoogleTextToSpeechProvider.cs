using Innoactive.Hub.SDK;
using System.Collections.Generic;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// Uses the Google text to speech api
    /// </summary>
    public class GoogleTextToSpeechProvider : WebTextToSpeechProvider
    {
        private const string URL = "https://www.google.com/speech-api/v1/synthesize?ie=UTF-8&text={0}&lang={1}&sv={2}&vn=rjs&speed=0.4";

        public GoogleTextToSpeechProvider() : base() { }

        public GoogleTextToSpeechProvider(IHttpProvider httpProvider) : base(httpProvider) { }

        public GoogleTextToSpeechProvider(IHttpProvider httpProvider, IAudioConverter audioConverter) : base(httpProvider, audioConverter) { }

        protected override string GetAudioFileDownloadUrl(string text)
        {
            return string.Format(URL, text, Configuration.Language, Configuration.Voice);
        }
    }
}