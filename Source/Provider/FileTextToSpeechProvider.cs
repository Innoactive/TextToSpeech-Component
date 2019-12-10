using UnityEngine;
using Innoactive.Hub.SDK;
using System;
using System.IO;
using System.Collections;
using Common.Logging;
using Innoactive.Hub.Threading;
using UnityEngine.Networking;

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
        private static readonly ILog logger = Logging.LogManager.GetLogger<FileTextToSpeechProvider>();

        protected readonly ITextToSpeechProvider FallbackProvider;

        protected readonly IAudioConverter AudioConverter = new NAudioConverter();

        protected TextToSpeechConfig Config;

        public FileTextToSpeechProvider(ITextToSpeechProvider fallbackProvider, TextToSpeechConfig config)
        {
            Config = config;
            FallbackProvider = fallbackProvider;
        }

        public FileTextToSpeechProvider(ITextToSpeechProvider fallbackProvider, IAudioConverter audioConverter, TextToSpeechConfig config)
        {
            Config = config;
            AudioConverter = audioConverter;
            FallbackProvider = fallbackProvider;
        }

        /// <inheritdoc/>
        public IAsyncTask<AudioClip> ConvertTextToSpeech(string text)
        {
            return new AsyncTask<AudioClip>(task =>
            {
                string filename = Config.GetUniqueTextToSpeechFilename(text);
                string path = GetPathToFile(filename);
                if (File.Exists(path))
                {
                    try
                    {
                        CoroutineDispatcher.Instance.StartCoroutine(LoadAudioFromFile(path, task));
                    }
                    catch (Exception ex)
                    {
                        task.InvokeOnError(ex);
                    }

                    return null;
                }
                else
                {
                    IAsyncTask<AudioClip> downloadTask = FallbackProvider.ConvertTextToSpeech(text);
                    downloadTask.OnError(task.InvokeOnError);
                    downloadTask.OnFinished(audio =>
                    {
                        if (Config.SaveAudioFilesToStreamingAssets)
                        {
                            // Ensure target directory exists.
                            string directory = Path.GetDirectoryName(path);
                            if (string.IsNullOrEmpty(directory) == false && Directory.Exists(directory) == false)
                            {
                                Directory.CreateDirectory(directory);
                            }

                            AudioConverter.TryWriteAudioClipToFile(audio, path);
                        }

                        task.InvokeOnFinished(audio);
                    });

                    return downloadTask.Execute();
                }
            });
        }

        /// <inheritdoc/>
        public void SetConfig(TextToSpeechConfig config)
        {
            Config = config;
        }

        protected virtual string GetPathToFile(string filename)
        {
            string directory = Application.dataPath + "/StreamingAssets/" + Config.StreamingAssetCacheDirectoryName;
            return directory + "/" + filename;
        }

        private IEnumerator LoadAudioFromFile(string path, IAsyncTask<AudioClip> task)
        {
#if UNITY_2017_3_OR_NEWER
            string url = UnityWebRequest.EscapeURL(path);
#else
            string url = WWW.EscapeURL(path);
#endif

            url = "file:///" + url;

            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);

            yield return request.SendWebRequest();

            if (request.isNetworkError == false && request.isHttpError == false)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                if (clip == null)
                {
                    logger.ErrorFormat("Could not load AudioClip '{0}' - AudioClip is null.", path);
                    task.InvokeOnError(new CouldNotLoadAudioFileException("Loading AudioClip from disk failed!"));
                }
                else
                {
                    task.InvokeOnFinished(clip);
                }
            }
            else
            {
                logger.ErrorFormat("Could not load AudioClip '{0}': {1}", path, request.error);
                task.InvokeOnError(new CouldNotLoadAudioFileException("Loading AudioClip from disk failed!"));
            }
        }

        public class CouldNotLoadAudioFileException : Exception
        {
            public CouldNotLoadAudioFileException(string msg) : base(msg) { }
            public CouldNotLoadAudioFileException(string msg, Exception ex) : base(msg, ex) { }
        }
    }
}