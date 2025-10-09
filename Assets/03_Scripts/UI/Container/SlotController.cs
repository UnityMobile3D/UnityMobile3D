using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotController : MonoBehaviour
{

    [SerializeField] private List<Slot> m_listSkillSlot = new List<Slot>();
    private SOEntryUI m_pSOTemporary = null;

    ///[SerializeField] private List<SOItem
   
    public void ActiveSkillSlot(SOEntryUI _pSOEntry)
    {
        m_pSOTemporary = _pSOEntry;
        uint iUICode = _pSOEntry.GetUIHashCode();
        foreach (Slot pSlot in m_listSkillSlot)
        {
           uint iSlotUICode = pSlot.GetSlotHashCode();
            pSlot.SetRaycast(false);

            if (iUICode == iSlotUICode)
                pSlot.ActiveSlot();
        }
    }


    public void SelectSlot(Slot _pSlot)
    {
        uint iUICode = m_pSOTemporary.GetUIHashCode();
        uint iSlotUICode = _pSlot.GetSlotHashCode();
        
        if (iUICode == iSlotUICode)
            _pSlot.Bind(m_pSOTemporary);

        foreach (Slot pSlot in m_listSkillSlot)
        {
            pSlot.SetRaycast(true);

            if (iUICode == iSlotUICode)
                pSlot.UnActiveSlot();
        }

        m_pSOTemporary = null;
    }
}
