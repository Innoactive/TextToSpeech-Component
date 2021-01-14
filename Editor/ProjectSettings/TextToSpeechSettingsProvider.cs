﻿using Innoactive.Creator.TextToSpeech;
using Innoactive.CreatorEditor.TextToSpeech.UI;
using Innoactive.CreatorEditor.UI;
using UnityEditor;
using UnityEngine;

internal class TextToSpeechSettingsProvider : BaseSettingsProvider
{
    const string Path = "Project/Creator/Text to Speech";

    public TextToSpeechSettingsProvider() : base(Path, SettingsScope.Project) {}

    protected override void InternalDraw(string searchContext)
    {
        GUILayout.Label("Configuration for your Text to Speech provider.", CreatorEditorStyles.ApplyPadding(CreatorEditorStyles.Label, 0));
        
        GUILayout.Space(8);
        
        TextToSpeechConfiguration config = TextToSpeechConfiguration.Instance;
        Editor.CreateEditor(config, typeof(TextToSpeechConfigurationEditor)).OnInspectorGUI();

        GUILayout.Space(8);
        
        CreatorGUILayout.DrawLink("Need Help? Visit our documentation", "https://developers.innoactive.de/documentation/creator/latest/articles/developer/12-text-to-speech.html", 0);
    }

    public override void OnDeactivate()
    {
        if (EditorUtility.IsDirty(TextToSpeechConfiguration.Instance))
        {
            TextToSpeechConfiguration.Instance.Save();
        }
    }

    [SettingsProvider]
    public static SettingsProvider Provider()
    {
        SettingsProvider provider = new TextToSpeechSettingsProvider();
        return provider;
    }
}
