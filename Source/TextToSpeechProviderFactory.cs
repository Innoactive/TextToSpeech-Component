using Innoactive.Hub.Config;
using Innoactive.Hub.Unity;
using System;
using System.Collections.Generic;
using Common.Logging;
using Innoactive.Hub.Training.TextToSpeech;
using LogManager = Innoactive.Hub.Logging.LogManager;

namespace Innoactive.Hub.TextToSpeech
{
    /// <summary>
    /// This factory creates and provides TextToSpeech provider.
    /// They are chosen by name, from the beginning there are two
    /// provider registered:
    /// - WatsonTextToSpeechProvider
    /// - GoogleTextToSpeechProvider
    /// </summary>
    public class TextToSpeechProviderFactory : Singleton<TextToSpeechProviderFactory>
    {
        private static readonly ILog logger = LogManager.GetLogger<WebTextToSpeechProvider>();

        public interface ITextToSpeechCreator
        {
            ITextToSpeechProvider Create(TextToSpeechConfig config);
        }

        /// <summary>
        /// Easy basic creator which requires an empty constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class BaseCreator<T> : ITextToSpeechCreator where T : ITextToSpeechProvider, new()
        {
            public ITextToSpeechProvider Create(TextToSpeechConfig config)
            {
                T provider = new T();
                provider.SetConfig(config);
                return provider;
            }
        }

        private readonly Dictionary<string, ITextToSpeechCreator> registeredProvider = new Dictionary<string, ITextToSpeechCreator>();

        public TextToSpeechProviderFactory()
        {
            RegisterProvider<WatsonTextToSpeechProvider>();
            RegisterProvider<GoogleTextToSpeechProvider>();
            RegisterProvider<MicrosoftSapiTextToSpeechProvider>();
        }

        /// <summary>
        /// Add or overwrites an provider of type T.
        /// </summary>
        public void RegisterProvider<T>() where T : ITextToSpeechProvider, new()
        {
            registeredProvider.Add(typeof(T).Name, new BaseCreator<T>());
        }

        /// <summary>
        ///  Creates an provider, always loads the actual text to speech config to set it up.
        /// </summary>
        public ITextToSpeechProvider CreateProvider()
        {
            TextToSpeechConfig config = new TextToSpeechConfig();
            if (config.Exists() == true)
            {
                config = config.Load();
                return CreateProvider(config);
            }
            throw new NoConfigurationFoundException("No TextToSpeechConfig found!");
        }

        /// <summary>
        /// Creates an provider with given config.
        /// </summary>
        public ITextToSpeechProvider CreateProvider(TextToSpeechConfig config)
        {
            if (registeredProvider.ContainsKey(config.Provider))
            {
                ITextToSpeechProvider provider = registeredProvider[config.Provider].Create(config);
                if (config.UseStreamingAssetFolder)
                {
                    logger.Info("Use streaming assets is set true, adding FileTextToSpeechProvider");
                    provider = new FileTextToSpeechProvider(provider, config);
                }
                return provider;
            }
            throw new NoMatchingProviderFoundException(string.Format("No matching provider with name '{0}' found!", config.Provider));
        }

        public class NoMatchingProviderFoundException : Exception
        {
            public NoMatchingProviderFoundException(string msg) : base (msg) { }
        }

        public class NoConfigurationFoundException : Exception
        {
            public NoConfigurationFoundException(string msg) : base(msg) { }
        }
    }
}