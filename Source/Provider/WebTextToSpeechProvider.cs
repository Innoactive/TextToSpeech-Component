using System;
using UnityEngine;
using UnityEngine.Networking;
using Innoactive.Hub.SDK;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// Abstract WebTextToSpeechProvider which can be used for web based provider.
    /// </summary>
    public abstract class WebTextToSpeechProvider : ITextToSpeechProvider
    {
        protected TextToSpeechConfiguration Configuration;

        protected readonly IHttpProvider HttpProvider;

        protected readonly IAudioConverter AudioConverter;

        protected WebTextToSpeechProvider() : this(new DotNetWebRequestHttpProvider()) { }

        protected WebTextToSpeechProvider(IHttpProvider httpProvider) : this(httpProvider, new NAudioConverter()) { }

        protected WebTextToSpeechProvider(IHttpProvider httpProvider, IAudioConverter audioConverter)
        {
            HttpProvider = httpProvider;
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
                DownloadAudio(text, task);
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
        protected virtual void DownloadAudio(string text, IAsyncTask<AudioClip> task)
        {
            IHttpRequest request = CreateRequest(GetAudioFileDownloadUrl(text), text);
            HttpProvider.Send<byte[]>(request)
                .OnFinished((result) =>
                {
                    byte[] data = result.Data;

                    if (result.StatusCode != 200 || data.Length == 0)
                    {
                        string errorMsg = $"Error while fetching audio from '{request.Url}' backend, code: '{result.StatusCode}'";
                        Debug.LogError(errorMsg);
                        task.InvokeOnError(new DownloadFailedException(errorMsg));
                    }
                    else
                    {
                        ParseAudio(data, task);
                    }
                })
                .OnError(task.InvokeOnError)
                .Execute();
        }

        /// <summary>
        /// Method to create the HttpRequest needed to get the file. If you have to add specific authorization or other
        /// header you can do it here.
        /// </summary>
        protected virtual IHttpRequest CreateRequest(string url, string text)
        {
            string escapedText = UnityWebRequest.EscapeURL(text);
            return new HttpRequest(new Uri(string.Format(url, escapedText)));
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
        }
    }
}