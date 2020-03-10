using System;
using System.Linq;
using UnityEditor;
using Innoactive.Hub.TextToSpeech;
using Innoactive.Hub.Training.Utils;
using Innoactive.Hub.Training.TextToSpeech;

/// <summary>
/// This class draws list of <see cref="ITextToSpeechProvider"/> in <see cref="textToSpeechConfiguration"/>.
/// </summary>
[CustomEditor(typeof(TextToSpeechConfiguration))]
public class TextToSpeechConfigurationEditor : Editor
{
    private TextToSpeechConfiguration textToSpeechConfiguration;
    private string[] providers = { "Empty" };
    private int providersIndex = 0;
    private int lastProviderSelectedIndex = 0;

    private void OnEnable()
    {
        textToSpeechConfiguration = (TextToSpeechConfiguration)target;
        providers = ReflectionUtils.GetConcreteImplementationsOf<ITextToSpeechProvider>().ToList().Select(type => type.Name).ToArray();
        lastProviderSelectedIndex = providersIndex = string.IsNullOrEmpty(textToSpeechConfiguration.Provider) ? Array.IndexOf(providers, typeof(MicrosoftSapiTextToSpeechProvider).Name) : Array.IndexOf(providers, textToSpeechConfiguration.Provider);
        textToSpeechConfiguration.Provider = providers[providersIndex];
    }

    /// <inheritdoc />
    public override void OnInspectorGUI()
    {
        providersIndex = EditorGUILayout.Popup("Provider", providersIndex, providers);
        DrawDefaultInspector();
    
        if (providersIndex != lastProviderSelectedIndex)
        {
            lastProviderSelectedIndex = providersIndex;
            textToSpeechConfiguration.Provider = providers[providersIndex];
        }
    }
}
