using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[CreateAssetMenu(menuName = "SO/NPC/Quest")]

[Serializable]
public class QuestReward
{
    public SOItem Item;    // 아이템 SO
    public int Amount;
}

[Serializable]
public enum eQuestType
{
    Kill,
    Collect,
    Talk
}

[Serializable]
public class QuestInfo
{
    public eQuestType Type;        
    public int TargetId;             // 몬스터ID, 아이템ID, 지역ID
    public int TargetCount;          // 목표 수량
}

public class SOQuestUI : SOEntryUI
{
    public LocalizedString QuestTitle;       // 퀘스트 제목
    public LocalizedString QuestDescription; // 퀘스트 설명

    public QuestInfo QuestInfo;
    public QuestReward QuestReward;

}
