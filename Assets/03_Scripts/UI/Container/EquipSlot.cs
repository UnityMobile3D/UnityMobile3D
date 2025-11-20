using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AddressableAssets.Build;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EquipSlot : Slot
{
    [SerializeField] private SOEquipUI m_pEquip = null;
    [SerializeField] private SOEquipUI m_pPreEquip = null;
    public SOEquipUI SOEquip { get => m_pEquip; }

    [SerializeField] private eEquipType m_eEquipType = eEquipType.None;

    private GameObject m_pEquipObject = null;

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

        m_pPreEquip = m_pEquip;

        if (_pSOTarget != null)
            m_pEquip = _pSOTarget as SOEquipUI;
        else
            m_pSOTarget = null;

        equiped();
    }

    public override void Using()
    {
        if (m_pEquip != null)
        {
            if (m_pEquipObject.TryGetComponent<Item>(out var pItem) == true)
                ItemEffectRunner.ApplyEffectUsing(m_pEquip.m_pSOItem, pItem.EffectContext);
        }

    }

    private void equiped()
    {
        if (m_pEquipObject != null)
        {
            if (m_pEquipObject.TryGetComponent<Item>(out var pPreItem) == true)
                ItemEffectRunner.ApplyEffectRelease(m_pPreEquip.m_pSOItem, pPreItem.EffectContext);
        }

        GameObject pEquip =
            ObjectPoolManager.m_Instance.GetObject(m_pEquip.m_pSOItem.ItemObject.AssetGUID, Vector3.zero, Vector3.zero);

        if (pEquip == null)
            return;

        m_pEquipObject = pEquip;
        if(pEquip.TryGetComponent<Item>(out var pItem) == true)
        {
            pItem.EffectContext.pTarget = GameManager.m_Instance.Player.gameObject;
            ItemEffectRunner.ApplyEffectEquipped(m_pEquip.m_pSOItem, pItem.EffectContext);
        }
    }

    private void selete_equip_slot()
    {
        DataService.m_Instance.TryDropDataAndSwap(m_pOwner, SlotIdx);
    }
}
