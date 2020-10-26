using Innoactive.Creator.TextToSpeech;
using Innoactive.CreatorEditor.TextToSpeech.UI;
using Innoactive.CreatorEditor.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

internal class TextToSpeechSettingsProvider : SettingsProvider
{
    const string Path = "Project/Creator/Text to Speech";

    public TextToSpeechSettingsProvider() : base(Path, SettingsScope.Project) {}

    public static bool IsSettingsAvailable()
    {
        return true;
    }

    public override void OnGUI(string searchContext)
    {
        GUILayout.Label("Configuration for your Text to Speech provider.", CreatorEditorStyles.ApplyPadding(CreatorEditorStyles.Label, 0));
        
        GUILayout.Space(8);
        
        TextToSpeechConfiguration config = TextToSpeechConfiguration.Instance;
        Editor.CreateEditor(config, typeof(TextToSpeechConfigurationEditor)).OnInspectorGUI();
        if (EditorUtility.IsDirty(config))
        {
            config.Save();
        }
        
        GUILayout.Space(8);
        
        CreatorGUILayout.DrawLink("Need Help? Visit our documentation", "https://developers.innoactive.de/documentation/creator/latest/articles/developer/12-text-to-speech.html", 0);
    }

    [SettingsProvider]
    public static SettingsProvider Provider()
    {
        if (IsSettingsAvailable())
        {
            SettingsProvider provider = new TextToSpeechSettingsProvider();
            return provider;
        }

        return null;
    }
}
