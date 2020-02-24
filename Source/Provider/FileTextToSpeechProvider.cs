using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Innoactive.Creator.IO;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// The disk based provider for text to speech, which is using the streaming assets folder.
    /// On the first step we check if the application has files provided on delivery.
    /// If there is no compatible file found, will download the file from the given
    /// fallback TextToSpeechProvider.
    /// </summary>
    public class FileTextToSpeechProvider : ITextToSpeechProvider
    {
        protected readonly ITextToSpeechProvider FallbackProvider;

        protected readonly IAudioConverter AudioConverter = new NAudioConverter();

        protected TextToSpeechConfiguration Configuration;

        public FileTextToSpeechProvider(ITextToSpeechProvider fallbackProvider, TextToSpeechConfiguration configuration)
        {
            Configuration = configuration;
            FallbackProvider = fallbackProvider;
        }

        public FileTextToSpeechProvider(ITextToSpeechProvider fallbackProvider, IAudioConverter audioConverter, TextToSpeechConfiguration configuration)
        {
            Configuration = configuration;
            AudioConverter = audioConverter;
            FallbackProvider = fallbackProvider;
        }

        /// <inheritdoc/>
        public async Task<AudioClip> ConvertTextToSpeech(string text)
        {
            string filename = Configuration.GetUniqueTextToSpeechFilename(text);
            string filePath = GetPathToFile(filename);
            AudioClip audioClip;
            
            if (FileManager.StreamingAssetsFileExists(filePath))
            {
                byte[] bytes = await FileManager.RetrieveFileFromStreamingAssets(filePath);
                float[] sound = TextToSpeechUtils.ShortsInByteArrayToFloats(bytes);

                audioClip = AudioClip.Create(text, channels: 1, frequency: 48000, lengthSamples: sound.Length, stream: false);
                audioClip.SetData(sound, 0);
            }
            else
            {
                audioClip = await FallbackProvider.ConvertTextToSpeech(text);

                if (Configuration.SaveAudioFilesToStreamingAssets)
                {
                    // Ensure target directory exists.
                    string directory = Path.GetDirectoryName(filePath);
                    
                    if (string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                    {
                        Directory.CreateDirectory(directory);
                    }

                    AudioConverter.TryWriteAudioClipToFile(audioClip, filePath);
                }
            }

            if (audioClip == null)
            {
                throw new CouldNotLoadAudioFileException("AudioClip is null.");
            }
            
            return audioClip;
        }

        /// <inheritdoc/>
        public void SetConfig(TextToSpeechConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected virtual string GetPathToFile(string filename)
        {
            string directory = $"{Configuration.StreamingAssetCacheDirectoryName}/{filename}";
            return directory;
        }
        
        public class CouldNotLoadAudioFileException : Exception
        {
            public CouldNotLoadAudioFileException(string msg) : base(msg) { }
            public CouldNotLoadAudioFileException(string msg, Exception ex) : base(msg, ex) { }
        }
    }
}