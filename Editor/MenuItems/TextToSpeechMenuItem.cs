using Innoactive.Creator.Core.Behaviors;
using Innoactive.Creator.Core.Internationalization;
using UnityEngine;
using Innoactive.Creator.TextToSpeech.Audio;
using Innoactive.CreatorEditor.UI;

namespace Innoactive.CreatorEditor.TextToSpeech.Behaviors
{
    public class TextToSpeechMenuItem : StepInspectorMenu.Item<IBehavior>
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
