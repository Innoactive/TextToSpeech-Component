using System;
using Innoactive.Creator.TextToSpeech;
using Innoactive.CreatorEditor.UI;
using UnityEditor;
using UnityEngine;

namespace Innoactive.CreatorEditor.TextToSpeech.UI.ProjectSettings
{
    public class TextToSpeechSectionProvider : IProjectSettingsSection
    {
        public string Title { get; } = "Text to Speech";
        public Type TargetPageProvider { get; } = typeof(LanguageSettingsProvider);
        public int Priority { get; } = 0;
        public void OnGUI(string searchContext)
        {
            GUILayout.Label("Configuration for your Text to Speech provider.", CreatorEditorStyles.ApplyPadding(CreatorEditorStyles.Label, 0));
        
            GUILayout.Space(8);
        
            TextToSpeechConfiguration config = TextToSpeechConfiguration.Instance;
            Editor.CreateEditor(config, typeof(TextToSpeechConfigurationEditor)).OnInspectorGUI();

            GUILayout.Space(8);
        
            CreatorGUILayout.DrawLink("Need Help? Visit our documentation", "https://developers.innoactive.de/documentation/creator/latest/articles/developer/12-text-to-speech.html", 0);
        }
        
        ~TextToSpeechSectionProvider()
        {
            if (EditorUtility.IsDirty(TextToSpeechConfiguration.Instance))
            {
                TextToSpeechConfiguration.Instance.Save();
            }
        }
    }
}