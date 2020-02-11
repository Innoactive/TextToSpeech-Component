using System;
using System.Collections;
using System.IO;
using System.Threading;
using Common.Logging;
using Innoactive.Hub.SDK;
using Innoactive.Hub.TextToSpeech;
using Innoactive.Hub.Threading;
using Innoactive.Hub.Training.Utils;
using UnityEngine;
using SpeechLib;
using LogManager = Innoactive.Hub.Logging.LogManager;
using ThreadPriority = System.Threading.ThreadPriority;

namespace Innoactive.Hub.Training.TextToSpeech
{
    /// <summary>
    /// Training TTS provider which uses Microsoft SAPI to generate audio.
    /// TextToSpeechConfig.Voice has to be either "male", "female", or "neutral".
    /// TextToSpeechConfig.Language is a language code ("de" or "de-DE" for German, "en" or "en-US" for English).
    /// It runs the TTS synthesis in a separate thread, saving the result to a temporary cache file.
    /// </summary>
    public class MicrosoftSapiTextToSpeechProvider : ITextToSpeechProvider
    {
        private static readonly ILog logger = LogManager.GetLogger<MicrosoftSapiTextToSpeechProvider>();

        private TextToSpeechConfiguration configuration;

        /// <summary>
        /// This is the template of the Speech Synthesis Markup Language (SSML) string used to change the language and voice.
        /// The first argument is the preferred language code (Examples: "de" or "de-DE" for German, "en" or "en-US" for English). If the language is not installed on the system, it chooses English.
        /// The second argument is the preferred gender of the voice ("male", "female", or "neutral"). If it is not installed, it chooses another gender.
        /// The third argument is a string which is read out loud.
        /// </summary>
        private const string ssmlTemplate = "<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='{0}'><voice languages='{0}' gender='{1}' required='languages' optional='gender'>{2}</voice></speak>";

        /// <summary>
        /// Remove the file at path and remove empty folders.
        /// </summary>
        private static void ClearCache(string path)
        {
            File.Delete(path);

            while (string.IsNullOrEmpty(path) == false)
            {
                path = Directory.GetParent(path).ToString();

                if (Directory.Exists(path) && Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0)
                {
                    Directory.Delete(path);
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// The result comes in byte array, but there are actually short values inside (ranged from short.Min to short.Max).
        /// </summary>
        private static float[] ShortsInByteArrayToFloats(byte[] shorts)
        {
            float[] floats = new float[shorts.Length / 2];

            for (int i = 0; i < floats.Length; i++)
            {
                short restoredShort = (short) ((shorts[i * 2 + 1] << 8) | (shorts[i * 2]));
                floats[i] = restoredShort / (float) short.MaxValue;
            }

            return floats;
        }

        /// <summary>
        /// When the speech is generated in a separate tread, there are clicking sounds at the beginning and at the end of audio data.
        /// </summary>
        private static float[] RemoveArtifacts(float[] floats)
        {
            // Empirically determined values.
            const int elementsToRemoveFromStart = 5000;
            const int elementsToRemoveFromEnd = 10000;

            float[] cleared = new float[floats.Length - elementsToRemoveFromStart - elementsToRemoveFromEnd];

            Array.Copy(floats, elementsToRemoveFromStart, cleared, 0, floats.Length - elementsToRemoveFromStart - elementsToRemoveFromEnd);

            return cleared;
        }

        /// <summary>
        /// Set up a file stream by path.
        /// </summary>
        private static SpFileStream PrepareFileStreamToWrite(string path)
        {
            SpFileStream stream = new SpFileStream();
            SpAudioFormat format = new SpAudioFormat();
            format.Type = SpeechAudioFormatType.SAFT48kHz16BitMono;
            stream.Format = format;
            stream.Open(path, SpeechStreamFileMode.SSFMCreateForWrite, true);

            return stream;
        }

        /// <inheritdoc />
        public void SetConfig(TextToSpeechConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <inheritdoc />
        public IAsyncTask<AudioClip> ConvertTextToSpeech(string text)
        {
            return new AsyncTask<AudioClip>(task =>
            {
                CoroutineDispatcher.Instance.StartCoroutine(GetAudio(task, text));
                return null;
            });
        }

        /// <summary>
        /// Coroutine that handles creation of an AudioClip from a <paramref name="text"/>.
        /// </summary>
        private IEnumerator GetAudio(IAsyncTask<AudioClip> task, string text)
        {
            string fullPath = PrepareFilepathForText(text);

            SpVoice synthesizer = new SpVoice();

            // Despite the fact that SpVoice.AudioOutputStream accepts values of type ISpeechBaseStream,
            // the single type of a stream that is actually working is a SpFileStream.
            SpFileStream stream = PrepareFileStreamToWrite(fullPath);

            synthesizer.AudioOutputStream = stream;

            // Try to get a valid two-letter ISO language code using the provided language in the configuration.
            string twoLetterIsoCode;
            if (configuration.Language.TryConvertToTwoLetterIsoCode(out twoLetterIsoCode) == false)
            {
                // If it fails, use English as default language.
                twoLetterIsoCode = "en";
                logger.Warn(string.Format("The language \"{0}\" given in the training configuration is not valid. It was changed to default: \"en\".", configuration.Language));
            }

            // Check the validity of the voice in the configuration.
            // If it is invalid, change it to neutral.
            string genderOfVoice = configuration.Voice.ToLower();
            switch (configuration.Voice.ToLower())
            {
                case "female":
                    break;
                case "male":
                    break;
                default:
                    genderOfVoice = "neutral";
                    break;
            }

            float[] sound = null;
            Thread synthesizingThread = new Thread(() =>
            {
                string ssmlText = string.Format(ssmlTemplate, twoLetterIsoCode, genderOfVoice, text);
                synthesizer.Speak(ssmlText, SpeechVoiceSpeakFlags.SVSFIsXML);
                synthesizer.WaitUntilDone(-1);
                stream.Close();

                byte[] bytes = File.ReadAllBytes(fullPath);

                float[] floats = ShortsInByteArrayToFloats(bytes);

                sound = RemoveArtifacts(floats);
            });

            synthesizingThread.Priority = ThreadPriority.Lowest;

            synthesizingThread.Start();

            while (synthesizingThread.ThreadState == ThreadState.Running)
            {
                yield return null;
            }

            ClearCache(fullPath);

            AudioClip result = AudioClip.Create(text, channels: 1, frequency: 48000, lengthSamples: sound.Length, stream: false);

            result.SetData(sound, 0);

            task.InvokeOnFinished(result);
        }

        /// <summary>
        /// Get a full path based on a <paramref name="text"/> to produce speech from, and create a directory for that.
        /// </summary>
        private string PrepareFilepathForText(string text)
        {
            string filename = Guid.NewGuid() + configuration.GetUniqueTextToSpeechFilename(text);
            string directory = Path.Combine(Application.temporaryCachePath.Replace('/', Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, configuration.StreamingAssetCacheDirectoryName);
            Directory.CreateDirectory(directory);
            return Path.Combine(directory, filename);
        }

        public static TextToSpeechConfiguration CreateConfig(string voice, string iso)
        {
            return new TextToSpeechConfiguration();
        }
    }
}
