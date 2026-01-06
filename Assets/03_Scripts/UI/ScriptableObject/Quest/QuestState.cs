using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestState : BaseUI, IContainer
{
    [SerializeField] private Container m_pQuestContainer = null;
    [SerializeField] private eContainerType m_eContainerType = eContainerType.QuestState;

    [SerializeField] private QuestDescInfo m_pQuestDescInfo;

    //Active
    private Dictionary<long, SOQuestUI> m_hashQuest;
    private Dictionary<long, SOQuestUI> m_hashCompletedQuest;
    
    protected override void Awake()
    {
        base.Awake();


        //컨테이너에서 스킬 눌렸다면 가져올 수 있게
        m_pQuestContainer.OnSelectEvt += select_quest;
    }


    public void Init()
    {
        m_pQuestContainer.SetParent(this);
        m_pQuestContainer.Build();
    }


    private void select_quest()
    {
        m_pQuestDescInfo.gameObject.SetActive(true);

        SlotView pTargetSlot = m_pQuestContainer.GetTargetSlot();
    }

    public void CloseTap()
    {
        gameObject.SetActive(false);
    }

            
    //[ 32        이벤트 타입            ][ 32           타겟 ID              ]
    public long GetHashCode(eQuestType _eType, int _iTargetID)
    {
        return (long)_eType<< 32 | (long)_iTargetID ;
    }

    //public IQuestEvent GetQuestEvent(eQuestType _eType)
    //{
    //    switch(_eType)
    //    {
    //        case eQuestType.Collect:
    //            {
    //                return new
    //            }
    //    }
    //}

    public void SetVisible(bool _bOn) { }
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }
    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0) { return null; }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0) { return 0; }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0) { return 0; }

    //데이터를 넣을 슬롯에 이미 데이터가 있는지 확인 후 있다면 기존 슬롯 데이터 정보를 반환
    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0) { return false; }
    //비어있는칸으로 넣기
    public bool AddData(SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        SOQuestUI pQuest = _pSOData as SOQuestUI;
        if (pQuest == null)
            return false;
        
        long lHashCode = GetHashCode(pQuest.QuestInfo.Type, pQuest.QuestInfo.TargetId);
        if(m_hashQuest.ContainsKey(lHashCode) == false && m_hashCompletedQuest.ContainsKey(lHashCode) == false)
        {
            m_hashQuest.Add(lHashCode, pQuest);
            m_pQuestContainer.AddData(pQuest);
            return true;

        }
        return false;

    }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iQuestID, int _iCategoryIdx = 0)
    {
        
        return false;
    }

    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0) { return false; }

    //무조건 타입을 가질 수 있도록
    public eContainerType ContainerType { get; }



}
