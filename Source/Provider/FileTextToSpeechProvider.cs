using System;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
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
        public async Task<AudioClip> ConvertTextToSpeech(string text)
        {
            string filename = Configuration.GetUniqueTextToSpeechFilename(text);
            string path = GetPathToFile(filename);
            AudioClip audioClip;
            
            if (File.Exists(path))
            {
                TaskCompletionSource<AudioClip> taskCompletion = new TaskCompletionSource<AudioClip>();
                CoroutineDispatcher.Instance.StartCoroutine(LoadAudioFromFile(path, taskCompletion));
                
                audioClip = await taskCompletion.Task;
            }
            else
            {
                audioClip = await FallbackProvider.ConvertTextToSpeech(text);

                if (Configuration.SaveAudioFilesToStreamingAssets)
                {
                    // Ensure target directory exists.
                    string directory = Path.GetDirectoryName(path);
                    
                    if (string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                    {
                        Directory.CreateDirectory(directory);
                    }
        
                    AudioConverter.TryWriteAudioClipToFile(audioClip, path);
                }
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
            string directory = $"{Application.streamingAssetsPath}/{Configuration.StreamingAssetCacheDirectoryName}/{filename}";
            return directory;
        }

        private IEnumerator LoadAudioFromFile(string path, TaskCompletionSource<AudioClip> taskCompletion)
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
                    taskCompletion.SetResult(clip);
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