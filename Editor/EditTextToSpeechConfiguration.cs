using UnityEditor;
using UnityEngine;

namespace Innoactive.Creator.TextToSpeech
{
    /// <summary>
    /// This class adds an entry in Creator's Utilities for showing project's <see cref="TextToSpeechConfiguration"/>.
    /// </summary>
    public class EditTextToSpeechConfiguration : MonoBehaviour
    {
        [MenuItem("Innoactive/Settings/TextToSpeech Settings", false, 12)]
        private static void EditTTSConfiguration()
        {
            Selection.activeObject = TextToSpeechConfiguration.LoadConfiguration();
        }
    }
}
