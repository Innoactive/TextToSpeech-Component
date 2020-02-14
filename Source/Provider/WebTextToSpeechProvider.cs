using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Innoactive.Hub.SDK;
using Innoactive.Hub.Threading;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// Abstract WebTextToSpeechProvider which can be used for web based provider.
    /// </summary>
    public abstract class WebTextToSpeechProvider : ITextToSpeechProvider
    {
        protected TextToSpeechConfiguration Configuration;

        protected readonly UnityWebRequest UnityWebRequest;

        protected readonly IAudioConverter AudioConverter;

        protected WebTextToSpeechProvider() : this(new UnityWebRequest()) { }

        protected WebTextToSpeechProvider(UnityWebRequest unityWebRequest) : this(unityWebRequest, new NAudioConverter()) { }

        protected WebTextToSpeechProvider(UnityWebRequest unityWebRequest, IAudioConverter audioConverter)
        {
            UnityWebRequest = unityWebRequest;
            AudioConverter = audioConverter;
        }

        #region Public Interface
        /// <inheritdoc/>
        public void SetConfig(TextToSpeechConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <inheritdoc/>
        public IAsyncTask<AudioClip> ConvertTextToSpeech(string text)
        {
            return new AsyncTask<AudioClip>((task) =>
            {
                CoroutineDispatcher.Instance.StartCoroutine(DownloadAudio(text, task));
                return null;
            });
        }
        #endregion

        #region Download handling
        /// <summary>
        /// Creates the specific url for the given voice, language and text to download the voice file.
        /// </summary>
        /// <param name="text">The text that should be converted into an audio file.</param>
        /// <returns>The full url required to receive the audio file for the given text message</returns>
        protected abstract string GetAudioFileDownloadUrl(string text);

        /// <summary>
        /// This method should asynchronous download the audio file to an AudioClip and call task OnFinish with it.
        /// You can use the ParseAudio method to convert the file (mp3) into an AudioClip.
        /// </summary>
        protected virtual IEnumerator DownloadAudio(string text, IAsyncTask<AudioClip> task)
        {
            using (UnityWebRequest request = CreateRequest(GetAudioFileDownloadUrl(text), text))
            {
                // Request and wait for the response.
                yield return request.SendWebRequest();

                if (request.isNetworkError == false && request.isHttpError == false)
                {
                    byte[] data = request.downloadHandler.data;

                    if (data == null || data.Length == 0)
                    {
                        string errorMsg = $"Error while retrieving audio: '{request.error}'";
                        
                        Debug.LogError(errorMsg);
                        task.InvokeOnError(new DownloadFailedException(errorMsg));
                    }
                    else
                    {
                        ParseAudio(data, task);
                    }
                }
                else
                {
                    string errorMsg = $"Error while fetching audio from '{request.uri}' backend, error: '{request.error}'";
                        
                    Debug.LogError(errorMsg);
                    task.InvokeOnError(new DownloadFailedException(errorMsg));
                }
            }
        }

        /// <summary>
        /// Method to create the UnityWebRequest needed to get the file.
        /// If you have to add specific authorization or other header you can do it here.
        /// </summary>
        protected virtual UnityWebRequest CreateRequest(string url, string text)
        {
            string escapedText = UnityWebRequest.EscapeURL(text);
            Uri uri = new Uri(string.Format(url, escapedText));
            
            return UnityWebRequest.Get(uri);
        }

        /// <summary>
        /// This method converts an mp3 file from byte to an AudioClip. If you have a different format, override this method.
        /// </summary>
        protected virtual AudioClip CreateAudioClip(byte[] data)
        {
            return AudioConverter.CreateAudioClipFromMp3(data);
        }
        #endregion

        private void ParseAudio(byte[] data, IAsyncTask<AudioClip> task)
        {
            try
            {
                AudioClip clip = CreateAudioClip(data);
                
                if (clip.loadState == AudioDataLoadState.Loaded)
                {
                    task.InvokeOnFinished(clip);
                }
                else
                {
                    task.InvokeOnError(new UnableToParseAudioFormatException("Creating AudioClip failed for text"));
                }
            }
            catch (Exception ex)
            {
                task.InvokeOnError(ex);
            }
        }

        public class DownloadFailedException : Exception
        {
            public DownloadFailedException(string msg) : base(msg) { }
            
            public DownloadFailedException(string msg, Exception ex) : base(msg, ex) { }
        }
    }
}