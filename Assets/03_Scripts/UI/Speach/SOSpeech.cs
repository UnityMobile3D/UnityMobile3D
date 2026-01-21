using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "SO/NPC/Speech")]
public class SOSpeech : ScriptableObject
{
    public LocalizedString Message;

    public SpeechChoice Choice;
}

[Serializable]
public enum eSpeechAction
{
    Store,
    Quest,
    End,
}


[Serializable]
public class SpeechChoice
{
    public LocalizedString PositiveText = null;        // 선택지에 표시될 문장
    public LocalizedString NegativeText = null;

    public SOSpeech NextSpeech;               // 이 선택지 선택 후 넘어갈 다음 노드(없으면 대화 종료)
    public eSpeechAction Action;               // 긍정적 대답에 대한 행동
}