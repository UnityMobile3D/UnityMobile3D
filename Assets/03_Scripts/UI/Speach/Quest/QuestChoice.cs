using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestChoice : MonoBehaviour
{
    [SerializeField] private Container m_pConatiner;
    private List<SONPCSpeech> m_listNPCSpeech = null;
    private void Awake()
    {
        m_pConatiner.OnSelectEvt += speech;
    }

    private void speech()
    {
        SlotView pSlot = m_pConatiner.GetTargetSlot();
        
        if(pSlot == null)
            gameObject.SetActive(false);
        else
        {
            int iTargetIdx = pSlot.SlotIdx;
            SpeechManager.m_Instance.ShowText(m_listNPCSpeech[iTargetIdx]);
        }

        m_listNPCSpeech = null;
    }

    public void ShowQuest(List<SONPCSpeech> _listNPCSPeech)
    {
        gameObject.SetActive(true);
        //기존 데이터 지우고 새로운 데이터로
        m_listNPCSpeech = _listNPCSPeech;

        m_pConatiner.ClearData();
        for(int i = 0; i< m_listNPCSpeech.Count; ++i)
            m_pConatiner.AddData(m_listNPCSpeech[i].SpeechInfo);

        m_pConatiner.BindData();
    }

}
