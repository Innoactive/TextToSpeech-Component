using System;
using UnityEngine;
using Innoactive.Creator.Core.Audio;
using Innoactive.Creator.Core.Behaviors;
using Innoactive.Creator.TextToSpeech.Audio;
using Innoactive.CreatorEditor.UI.Drawers;
using Newtonsoft.Json;

namespace Innoactive.CreatorEditor.TextToSpeech.UI.Drawers
{
    /// <summary>
    /// Default drawer for <see cref="PlayAudioBehavior"/>. It changes displayed name to "Play TTS Audio" or "Play Audio File", depending on which AudioData is used.
    /// </summary>
    [DefaultTrainingDrawer(typeof(PlayAudioBehavior.EntityData))]
    public class PlayTTSAudioBehaviorDrawer : PlayAudioBehaviorDrawer
    {
        /// <inheritdoc />
        protected override GUIContent GetTypeNameLabel(object value, Type declaredType)
        {
            PlayAudioBehavior.EntityData behavior = value as PlayAudioBehavior.EntityData;

            if (behavior == null)
            {
                return base.GetTypeNameLabel(value, declaredType);
            }

            if (behavior.AudioData is TextToSpeechAudio)
            {
                return new GUIContent("Play TTS Audio");
            }

            if (behavior.AudioData is ResourceAudio)
            {
                return new GUIContent("Play Audio File");
            }

            return base.GetTypeNameLabel(value, declaredType);
        }
    }
}

