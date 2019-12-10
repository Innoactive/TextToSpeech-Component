using System;

namespace Innoactive.Hub.Training.Audio
{
    [Obsolete("This interface is obsolete, please use Localization.Load() directly.")]
    public interface IAudioTextProvider
    {
        void LoadAudioText();
    }
}
