using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOItemUI;

[CreateAssetMenu(menuName = "UIData/Catalog/Equip UI", fileName = "SOEntryUI")]


public class SOEquipUI : SOEntryUI
{
  
    [SerializeField] private eEquipType equiptype;
    public eEquipType EquipType => equiptype;

    public SOItem m_pSOItem;

    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)equiptype << (int)SOEntryUI.eUIType.Equip;

        return iHashCode;
    }
}
