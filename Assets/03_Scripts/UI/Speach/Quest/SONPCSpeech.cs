using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "SO/NPC/NPCSpeech")]
public class SONPCSpeech : ScriptableObject
{
    public SOSpeech Speech;
    public SOSpeechInfoUI SpeechInfo;
}
