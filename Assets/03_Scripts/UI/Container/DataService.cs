using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public enum ContainerType
{
    None,
    Inventory,
    Equipment,
    End,
}

public interface IContainer
{
    public void SelectData(int _iDataIdx);
    public SOEntryUI GetData(int _iDataIdx);
    public int GetDataAmount(int _iDataIdx);
    public int GetDataAmount(SOEntryUI _pSoData);

    //데이터를 넣을 슬롯에 이미 데이터가 있는지 확인 후 있다면 기존 슬롯 데이터 정보를 반환
    public bool AddData(IContainer _IOtherContainer, int _iDataIdx, SOEntryUI _pSOData, int _iAmount);
    public bool DeleteData(int _iDataIdx);

    public bool FindData(SOEntryUI _pData);
    public bool FindData(int _iDataIdx);

    public bool Consume(int _iDataIdx, int _iAmount);


}

public struct SlotRef
{
    public IContainer Container; //보내려는 컨테이너
    public SOEntryUI Data; //보낼 데이터
    public int DataIdx; //보낼 컨테이너 데이터의 인덱스
    public int Amount; //개수
}

public class DataService : MonoBehaviour
{
    //1. 모든 데이터 관리(스킬, 아이템 장비..)
    //2. 데이터 이동(인벤->장비, 인벤->상점, 인벤->버리기)
    //3. 데이터 검색(아이템, 스킬)

    private static DataService m_instance = null;
    [SerializeField] List<IContainer> m_listEventTory = new((int)ContainerType.End);

    //아이엠 등록 (null가능 유니티 인스펙터에서 확인 불가)
    private SlotRef? m_pTargetSlot = null;

    //public void RegisterData()
    //등동된 데이터 Container로 가져오기
    //public void BringData()


    public static DataService Instance { get; private set; }

    private void Awake()
    {
      
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 최초 인스턴스 등록
        Instance = this;
        DontDestroyOnLoad(gameObject); // 선택: 씬 전환에도 유지
    }
    public bool Transfer(int _iAmount, IContainer _Ifrom, int _iFromIdx,
                                IContainer _Ito, int _iToIdx)
    {

        SOEntryUI pDataUI = _Ifrom.GetData(_iFromIdx);

        if (pDataUI == null || _Ifrom.GetDataAmount(_iFromIdx) < _iAmount) 
            return false;

        if (_Ito.AddData(_Ifrom, _iToIdx, pDataUI, _iAmount) == false)  
            return false;
        
        return true;
    }

    //이동 후 기존 컨테이너에서 삭제   
    public bool TransferAndDelete(int _iAmount, IContainer _Ifrom, int _iFromIdx,
                                IContainer _Ito, int _iToIdx = -1)
    {
        if (Transfer(_iAmount, _Ifrom, _iFromIdx, _Ito, _iToIdx) == false)
            return false;

        _Ifrom.DeleteData(_iFromIdx);

        return true;
    }


    public bool StartPickData(IContainer _pFrom, SOEntryUI _pEntryUI, int _iFromIdx, int _iAmount)
    {
        if (_pEntryUI == null)
            return false;

        m_pTargetSlot = new SlotRef
        {
            Container = _pFrom,
            Data = _pEntryUI,
            DataIdx = _iFromIdx,
            Amount = _iAmount
        };
        
        return true;
    }

    public bool TryDropData(IContainer _pTo, int _iToIdx)
    {
        if (m_pTargetSlot == null)
            return false;

        //어떤 컨테이너에서 지정한 인덱스(상대 컨테이너)로 어떤 데이터를 얼마만큼 보낼지
        if (_pTo.AddData(m_pTargetSlot.Value.Container, _iToIdx, m_pTargetSlot.Value.Data, m_pTargetSlot.Value.Amount) == false)
        {
            m_pTargetSlot = null;
            return false;
        }

        m_pTargetSlot.Value.Container.DeleteData(m_pTargetSlot.Value.DataIdx);
        m_pTargetSlot = null;

        return true;
    }

    //인벤 -> 슬롯 , 인벤-> 슬롯(중복)
    public bool TryDropDataAndSwap(IContainer _pTo, int _iToIdx)
    {
        if (m_pTargetSlot == null)
            return false;

        //데이터를 넣기 전에 해당 인덱스에 이미 데이터가 있다면 보관
        SOEntryUI pEntryUI = _pTo.GetData(_iToIdx);
        int iAmount = 1;

        if (pEntryUI)
        {
            //해당 데이터 미리 보관 후 삭제
            iAmount = _pTo.GetDataAmount(_iToIdx);
            _pTo.DeleteData(_iToIdx);
        }

        //어떤 컨테이너에서 지정한 인덱스(상대 컨테이너)로 어떤 데이터를 얼마만큼 보낼지
        if (_pTo.AddData(m_pTargetSlot.Value.Container, _iToIdx, m_pTargetSlot.Value.Data, m_pTargetSlot.Value.Amount) == false)
        {
            m_pTargetSlot = null;
            return false;
        }
        m_pTargetSlot.Value.Container.DeleteData(m_pTargetSlot.Value.DataIdx);

        //기존 데이터 넘겨주기
        if (pEntryUI != null)
        {
            m_pTargetSlot.Value.Container.AddData(_pTo, m_pTargetSlot.Value.DataIdx, pEntryUI, iAmount);
        }

        m_pTargetSlot = null;
       
        return true;
    }

    public SlotRef? GetTargetSlot()
    { 
        if (m_pTargetSlot == null)
            return null;

        return m_pTargetSlot.Value;
    }

    private void clear_target()
    {
        m_pTargetSlot.Value.Container.DeleteData(m_pTargetSlot.Value.DataIdx);
        m_pTargetSlot = null;
    }
}
