using Innoactive.Hub.Config;

namespace Innoactive.Hub.TextToSpeech
{
    public class TextToSpeechConfig : ConfigBase
    {
        public override string DirectoryPath
        {
            get
            {
                return "./Config";
            }
        }

        public string Provider;

        public string Language = null;

        public string Voice = null;

        /// <summary>
        /// Usage of the standard HTML cache.
        /// </summary>
        public bool UseCache = true;

        /// <summary>
        /// Enables the usage of the streaming asset folder as second cache directory to allow deliveries which work offline too.
        /// This means the StreamingAssets/{StreamingAssetCacheDirectoryName} will be searched first and only if there
        /// is no fitting audio file found the text to speech provider will be used to download the audio file from web.
        /// </summary>
        public bool UseStreamingAssetFolder = true;

        /// <summary>
        /// With this option enabled the application tries to save the all downloaded audio files to
        /// StreamingAssets/{StreamingAssetCacheDirectoryName}.
        /// </summary>
        public bool SaveAudioFilesToStreamingAssets = false;

        /// <summary>
        /// StreamingAsset directory name which is used to load/save audio files.
        /// </summary>
        public string StreamingAssetCacheDirectoryName = "TextToSpeech";

        public string Auth;
    }
}