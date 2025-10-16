using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static System.Net.Mime.MediaTypeNames;
using eSkillType = SOSkillUI.eSkillType;
using eUIType = SOEntryUI.eUIType;
using Image = UnityEngine.UI.Image;


//(사용 버튼)
public class Slot : ButtonUI
{
    protected SOEntryUI m_pSOTarget = null;
    public SOEntryUI SOTarget { get => m_pSOTarget; }

    protected IContainer m_pOwner = null;
    [SerializeField] protected ButtonUI m_pCheckUI = null;

    protected Image m_pCheckUIImage = null;
    [SerializeField] private Image m_pIcon = null;

    protected CoolDownView m_pCoolDownView = null;

    [Header("Slot Type")]
    [SerializeField] private eUIType m_eUIType = eUIType.None;

    protected uint m_iUIType = 0;
    protected bool m_bSlotActive = true;
    public bool IsActiveSlot { get => m_bSlotActive; }

    protected bool m_bCanUse = true;
    public bool IsCanUse { get => m_bCanUse;}

    [SerializeField] protected int m_iSlotIdx;
    public int SlotIdx { get => m_iSlotIdx; }

    //빼기 버튼 항상 넣어두고 +버튼 없이 그냥 스킬 누르고 빈 슬롯에 넣어주는 형식으로
    override protected void Awake()
    {
        base.Awake();

        m_pCheckUI.SetRaycast(false);
        m_pCheckUIImage = m_pCheckUI.GetComponent<Image>();
        m_pCheckUIImage.enabled = false;

        m_pOwner = GetComponentInParent<IContainer>();

        m_iUIType = (uint)m_eUIType;
    }

    private void OnDestroy()
    {
        
    }
    private void Update()
    {
      
    }

    public virtual void Bind(SOEntryUI _pSOTarget)
    {
        m_pSOTarget = _pSOTarget;

        if(m_pSOTarget == null)
            m_pIcon.enabled = false;
        else
        {
            m_pIcon.enabled = true;
            m_pIcon.sprite = _pSOTarget.Icon;
        }

        m_pIcon.color = Color.white;
    }
    public void ActiveSlot()
    {
        m_pCheckUI.SetRaycast(true);
        m_pCheckUIImage.enabled = true;
    }

    public void UnActiveSlot()
    {
        m_pCheckUI.SetRaycast(false);
        m_pCheckUIImage.enabled = false;
    }

    public uint GetSlotHashCode()
    {
        return m_iUIType;
    }


    public override void OnPointerClick(PointerEventData e)
    {
        if(m_pSOTarget != null && m_bCanUse != false)
        {
            //여기서 스킬, 아이템 사용
            Using();
        }
    }

    public virtual void Using()
    {
        //여기서 Effect 데이터를 매니저에 던지기 , 혹은 스킬이라면 skillslot에서 skillManager에게 던지기
    }


    public void SetCoolDownView(CoolDownView _pCoolDownView)
    {
        m_pCoolDownView = _pCoolDownView;
    }

    public void SetCoolTime(float _fCoolTime)
    { 
        if (m_pCoolDownView == null)
            return;

        m_pCoolDownView.SetCoolTime(_fCoolTime);
    }

    public void SetUse(bool _bCanUse)
    {
        m_bCanUse = _bCanUse;
        
        if(m_bCanUse == false)
            m_pIcon.color = Color.gray;
        else
            m_pIcon.color = Color.white;
    }

   

    public void SetSlotIdx(int _iIdx){m_iSlotIdx = _iIdx;}  
        

}



