using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
[CreateAssetMenu(menuName = "SO/NPC/QuestInfo")]
public class SOQuestInfo : ScriptableObject
{
    public eQuestType Type;
    public int TargetId;             // 몬스터ID, 아이템ID, 지역ID
    public int TargetAmount;          // 목표 수량
}


