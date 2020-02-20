﻿using UnityEngine;
using Innoactive.Hub.Training.Audio;
using Innoactive.Creator.Internationalization;
using Innoactive.Hub.Training.Editors.Configuration;

namespace Innoactive.Hub.Training.Behaviors.Editors
{
    public class TextToSpeechMenuItem : Menu.Item<IBehavior>
    {
        public override GUIContent DisplayedName
        {
            get
            {
                return new GUIContent("Audio/Play TextToSpeech Audio");
            }
        }

        public override IBehavior GetNewItem()
        {
            return new PlayAudioBehavior(new TextToSpeechAudio(new LocalizedString()), BehaviorExecutionStages.Activation, true);
        }
    }
}
