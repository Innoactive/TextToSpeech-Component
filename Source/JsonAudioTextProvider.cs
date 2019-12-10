using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Innoactive.Hub.Training.Audio
{
    
    [Obsolete("This interface is obsolete, please use Localization.Load() directly.")]
    public class JsonAudioTextProvider : IAudioTextProvider
    {
        private static readonly Common.Logging.ILog logger = Logging.LogManager.GetLogger<JsonAudioTextProvider>();

        private string path;

        public JsonAudioTextProvider(string path)
        {
            this.path = path;
        }

        public void LoadAudioText()
        {
            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                Localization.entries = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            }
            else
            {
                logger.ErrorFormat("Given file '{0}' seem not to exists.", path);
            }
        }
    }
}
