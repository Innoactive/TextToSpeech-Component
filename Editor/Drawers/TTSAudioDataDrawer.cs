﻿using System;
using System.Reflection;
using Innoactive.Creator.Core.Audio;
using Innoactive.Creator.Core.Internationalization;
using UnityEngine;
using Innoactive.Creator.TextToSpeech.Audio;
using Innoactive.CreatorEditor.Core.UI.Drawers;
using Innoactive.CreatorEditor.UI.Drawers;

namespace Innoactive.CreatorEditor.TextToSpeech.UI.Drawers
{
    /// <inheritdoc />
    [DefaultTrainingDrawer(typeof(IAudioData))]
    public class TTSAudioDataDrawer : AudioDataDrawer
    {
        /// <inheritdoc />
        public override Rect Draw(Rect rect, object currentValue, Action<object> changeValueCallback, GUIContent label)
        {
            ResourceAudio resourceAudio = currentValue as ResourceAudio;
            TextToSpeechAudio ttsAudio = currentValue as TextToSpeechAudio;

            if (resourceAudio != null)
            {
                if (resourceAudio.Path == null)
                {
                    resourceAudio.Path = new LocalizedString();
                    changeValueCallback(resourceAudio);
                }

                ITrainingDrawer pathDrawer = DrawerLocator.GetDrawerForMember(resourceAudio.GetType().GetProperty("Path", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), typeof(LocalizedString));
                return pathDrawer.Draw(rect, resourceAudio.Path, newPath =>
                {
                    resourceAudio.Path = (LocalizedString) newPath;
                    changeValueCallback(resourceAudio);
                }, label);
            }

            if (ttsAudio != null)
            {
                if (ttsAudio.Text == null)
                {
                    ttsAudio.Text = new LocalizedString();
                    changeValueCallback(ttsAudio);
                }

                ITrainingDrawer pathDrawer = DrawerLocator.GetDrawerForMember(ttsAudio.GetType().GetProperty("Text", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance), typeof(LocalizedString));
                return pathDrawer.Draw(rect, ttsAudio.Text, newPath =>
                {
                    ttsAudio.Text = (LocalizedString) newPath;
                    changeValueCallback(ttsAudio);
                }, label);
            }

            return base.Draw(rect, currentValue, changeValueCallback, label);
        }
    }
}
