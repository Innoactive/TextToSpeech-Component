using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Innoactive.Hub.Training.Configuration;

namespace Innoactive.Hub.TextToSpeech
{
    [CreateAssetMenu(fileName = "TextToSpeechConfiguration", menuName = "Innoactive/TextToSpeech", order = 1)]
    public class TextToSpeechConfiguration : ScriptableObject
    {
        /// <summary>
        /// Name of the <see cref="ITextToSpeechProvider"/>.
        /// </summary>
        public string Provider;

        /// <summary>
        /// Language which should be used.
        /// </summary>
        /// <remarks>It depends on the chosen provider.</remarks>
        public string Language;

        /// <summary>
        /// Voice that should be used.
        /// </summary>
        /// <remarks>It depends on the chosen provider.</remarks>
        public string Voice;

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

        /// <summary>
        /// Used to authenticate at the provider, if required.
        /// </summary>
        public string Auth;

        /// <summary>
        /// Loads an existing <see cref="TextToSpeechConfiguration"/>. If the <see cref="TextToSpeechConfiguration"/> does not exist in the project
        /// it creates and saves a new instance with default values.
        /// </summary>
        /// <remarks>New instances are not saved when they are created during runtime.</remarks>
        public static TextToSpeechConfiguration LoadConfiguration()
        {
            string filter = string.Format("t:{0}", typeof(TextToSpeechConfiguration).Name);
            string[] configsGUIDs = AssetDatabase.FindAssets(filter);

            if (configsGUIDs.Any())
            {
                string configFileGuid = configsGUIDs.First();
                string configFilePath = AssetDatabase.GUIDToAssetPath(configFileGuid);
                
                return Resources.Load<TextToSpeechConfiguration>(configFilePath);
            }
            
            TextToSpeechConfiguration textToSpeechConfiguration = CreateInstance<TextToSpeechConfiguration>();

            if (Application.isPlaying)
            {
                RuntimeConfigurator.Configuration.SetTextToSpeechConfiguration(textToSpeechConfiguration);
                Debug.LogWarning("No text to speech configuration found!\nTo create a new configuration go to Assets > Create > Innoactive > TextToSpeech.");
            }
            else
            {
                string resourcesPath = "Assets/Resources/";
                string configFilePath = string.Format("{0}{1}.asset", resourcesPath, typeof(TextToSpeechConfiguration).Name);
                
                if (Directory.Exists(resourcesPath) == false)
                {
                    Directory.CreateDirectory(resourcesPath);
                }
                
                AssetDatabase.CreateAsset(textToSpeechConfiguration, configFilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            return textToSpeechConfiguration;
        }
    }
}