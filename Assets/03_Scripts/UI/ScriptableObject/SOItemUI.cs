using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "UIData/Catalog/Item UI", fileName = "SOEntryUI")]
public class SOItemUI : SOEntryUI
{
    public enum eItemType
    {
        None,
        ConsumeItem,
    }

    [SerializeField] private uint level;
    [SerializeField] private eItemType itemtype;
    [SerializeField] private SOItemEffect[] effects;
    [SerializeField] private Values basevalues;

    //기본 정적 효과 값, (만약 크리티컬이나 이런 회복량 증가가 있다면 기본(정적) + 증가값(동적) 으로 효과 적용 
    public Values BaseValues => basevalues;
    public SOItemEffect[] Effects => effects;
    public uint Level => level;
    public eItemType ItemType => itemtype;


    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)itemtype << (int)SOEntryUI.eUIType.Item;

        return iHashCode;
    }
}
