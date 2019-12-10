using Innoactive.Hub.SDK;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// Uses the Watson text to speech api
    /// </summary>
    public class WatsonTextToSpeechProvider : WebTextToSpeechProvider
    {
        private const string URL = "https://stream.watsonplatform.net/text-to-speech/api/v1/synthesize?text={0}&voice={1}_{2}&accept=audio/mp3";

        public WatsonTextToSpeechProvider() : base() { }

        public WatsonTextToSpeechProvider(IHttpProvider httpProvider) : base(httpProvider) { }

        protected override IHttpRequest CreateRequest(string url, string text)
        {
            IHttpRequest request = base.CreateRequest(url, text);
            request.Headers["Authorization"] = Config.Auth;
            return request;
        }

        protected override string GetAudioFileDownloadUrl(string text)
        {
            return string.Format(URL, text, Config.Language, Config.Voice);
        }
    }
}