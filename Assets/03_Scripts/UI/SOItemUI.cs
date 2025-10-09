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
        EquipItem,
    }

    [SerializeField] private uint level;
    [SerializeField] private eItemType itemtype;
    [SerializeField] private SOItemEffect[] effects;

    public SOItemEffect[] Effects => effects;
    public uint Level => level;
    public eItemType ItemType => itemtype;


    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)itemtype << 16;

        return iHashCode;
    }
}
