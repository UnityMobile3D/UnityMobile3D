using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UIData/Catalog/Skill UI", fileName = "SOEntryUI")]
public class SOSkillUI : SOEntryUI
{
    public enum eSkillType
    {
        None,
        Default,
        SubSkill,
        MainSkill,
    }

    [SerializeField] private uint level;
    [SerializeField] private eSkillType skilltype;
    //[SerializeField] private uint cost;

    public uint Level => level;
    public eSkillType SkillType => skilltype;

    
    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)skilltype << (int)SOEntryUI.eUIType.Skill;

        return iHashCode;
    }
}
