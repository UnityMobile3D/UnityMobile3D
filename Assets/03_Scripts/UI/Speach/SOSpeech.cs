using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SOSpeech : ScriptableObject
{
    public string Message;

    public List<SpeechChoice> Choices;
}

[Serializable]
public class SpeechChoice
{
    public string ChoiceText;        // 선택지에 표시될 문장

    public SOSpeech NextSpeech;      // 이 선택지 선택 후 넘어갈 다음 노드(없으면 대화 종료)

    public UnityEvent OnSelected;    // 선택될 때 발생시킬 이벤트(옵션)
}