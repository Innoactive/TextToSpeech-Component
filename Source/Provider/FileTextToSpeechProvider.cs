using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Innoactive.Hub.Threading;

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
        public void ConvertTextToSpeech(string text, Action<AudioClip> OnFinished)
        {
            string filename = Configuration.GetUniqueTextToSpeechFilename(text);
            string path = GetPathToFile(filename);
            
            if (File.Exists(path))
            {
                CoroutineDispatcher.Instance.StartCoroutine(LoadAudioFromFile(path, OnFinished));
            }
            else
            {
                FallbackProvider.ConvertTextToSpeech(text, audio =>
                {
                    if (Configuration.SaveAudioFilesToStreamingAssets)
                    {
                        // Ensure target directory exists.
                        string directory = Path.GetDirectoryName(path);
                        
                        if (string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                        {
                            Directory.CreateDirectory(directory);
                        }
            
                        AudioConverter.TryWriteAudioClipToFile(audio, path);
                    }
                    
                    OnFinished.Invoke(audio);
                });
            }
        }

        /// <inheritdoc/>
        public void SetConfig(TextToSpeechConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected virtual string GetPathToFile(string filename)
        {
            string directory = $"{Application.streamingAssetsPath}/{Configuration.StreamingAssetCacheDirectoryName}/{filename}";
            return directory;
        }

        private IEnumerator LoadAudioFromFile(string path, Action<AudioClip> OnFinished)
        {
            string url = $"file:///{UnityWebRequest.EscapeURL(path)}";

            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);

            yield return request.SendWebRequest();

            if (request.isNetworkError == false && request.isHttpError == false)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                
                if (clip == null)
                {
                    Debug.LogErrorFormat("Could not load AudioClip '{0}' - AudioClip is null.", path);
                    throw new CouldNotLoadAudioFileException("Loading AudioClip from disk failed!");
                }
                else
                {
                    OnFinished.Invoke(clip);
                }
            }
            else
            {
                Debug.LogErrorFormat("Could not load AudioClip '{0}': {1}", path, request.error);
                throw new CouldNotLoadAudioFileException("Loading AudioClip from disk failed!");
            }
        }

        public class CouldNotLoadAudioFileException : Exception
        {
            public CouldNotLoadAudioFileException(string msg) : base(msg) { }
            public CouldNotLoadAudioFileException(string msg, Exception ex) : base(msg, ex) { }
        }
    }
}