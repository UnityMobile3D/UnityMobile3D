using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SOItemUI;
using static SOSkillUI;

public class ItemSlot : Slot
{
    [SerializeField] private SOItemUI m_pSOItem;

    [Header("Item Type")]
    [SerializeField] private eItemType m_eItemType = eItemType.None;

    [SerializeField] private TextMeshProUGUI m_pCountBadge = null;

    public SOItemUI SOItem { get => m_pSOItem; }

    public List<AdditionalEffect> m_listEffect = new List<AdditionalEffect>();

    protected override void Awake()
    {
        base.Awake();
        m_pCheckUI.OnClickEvt += selete_item_slot;

        m_iUIType |= (uint)m_eItemType << 16;
    }

    public override void Bind(SOEntryUI _pSOTarget)
    {
        base.Bind(_pSOTarget);

        m_pSOItem = _pSOTarget as SOItemUI;

        SetCoolTime(m_pSOItem.Cooldown);

        update_count();
    }

    public override void Using()
    {
        m_bCanUse = false;

        //Target을 여기서 셋팅

        ItemEffectRunner.UsingItem(m_pSOItem, null, null,m_listEffect);
    }
    private void selete_item_slot()
    {
        DataService.Instance.TryDropDataAndSwap(m_pOwner, SlotIdx);
    }

    private void update_count()
    {
        //데이터 사용 후 인덱스 업데이트
        m_pOwner?.Consume(m_iSlotIdx,1);

        int iCount = m_pOwner?.GetDataAmount(m_iSlotIdx) ?? 0;
        if(iCount == 0)
        {
            //삭제
        }

        bool bShow = iCount > 1; // 1개 이하면 보통 표기 안 함
        if (bShow)
            m_pCountBadge.enabled = bShow;

        if (bShow)
            m_pCountBadge.text = iCount.ToString();

    }
}

