using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOItemUI;

[CreateAssetMenu(menuName = "UIData/Catalog/Equip UI", fileName = "SOEntryUI")]

public class SOEquipUI : SOEntryUI
{
    public enum eEquipType
    {
        None,
        Hat,
        Top,
        Shoes,
        Weapon,
    }

    [SerializeField] private uint level;
    [SerializeField] private eEquipType equiptype;
    //[SerializeField] private SOItemEffect[] effects;
    [SerializeField] private Values basevalues;

    //기본 정적 효과 값, (만약 크리티컬이나 이런 회복량 증가가 있다면 기본(정적) + 증가값(동적) 으로 효과 적용 
    public Values BaseValues => basevalues;
    //public SOItemEffect[] Effects => effects;
    public uint Level => level;
    public eEquipType EquipType => equiptype;


    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)equiptype << (int)SOEntryUI.eUIType.Equip;

        return iHashCode;
    }
}
