using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using eEquipType = SOEquipUI.eEquipType;
public class EquipSlot : Slot
{
    [SerializeField] private SOEquipUI m_pSOEquip;

    [Header("Item Type")]
    [SerializeField] private eEquipType m_eEquipType = eEquipType.None;


    public SOEquipUI SOEquip { get => m_pSOEquip; }

    //public List<AdditionalEffect> m_listEffect = new List<AdditionalEffect>();

    protected override void Awake()
    {
        base.Awake();
        m_pCheckUI.OnClickEvt += selete_equip_slot;

        m_iUIType |= (uint)m_eEquipType << (int)SOEntryUI.eUIType.Equip;
    }

    public override void Bind(SOEntryUI _pSOTarget)
    {
        base.Bind(_pSOTarget);

        if (_pSOTarget != null)
            m_pSOEquip = _pSOTarget as SOEquipUI;
        else
            m_pSOTarget = null;
        
    }

    public override void Using()
    {
        
    }
    private void selete_equip_slot()
    {
        DataService.m_Instance.TryDropDataAndSwap(m_pOwner, SlotIdx);
    }
}
