using System;

namespace Innoactive.Hub.TextToSpeech
{
    public class UnableToParseAudioFormatException : Exception
    {
        public UnableToParseAudioFormatException(string msg) : base(msg) { }
    }
}
