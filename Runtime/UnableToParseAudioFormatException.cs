using System;

namespace Innoactive.Creator.TextToSpeech
{
    public class UnableToParseAudioFormatException : Exception
    {
        public UnableToParseAudioFormatException(string msg) : base(msg) { }
    }
}
