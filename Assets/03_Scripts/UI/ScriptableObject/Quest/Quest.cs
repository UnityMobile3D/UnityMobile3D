using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProgressEvent
{
    public SOQuestUI Quest;
    public int CurrentAmount = 0;
    public int Amount = 0;
}

public class Quest 
{
    //targetID, 
    Dictionary<int, ProgressEvent> m_hashQuestEvent = new();

    public void Add(SOQuestUI _pQuest, int _iTargetID, int _iAmount)
    { 
        if (m_hashQuestEvent.ContainsKey(_iTargetID) == false)
        {
            ProgressEvent pEvent = new ProgressEvent();
            pEvent.Amount = _iAmount;
            pEvent.Quest = _pQuest;
            m_hashQuestEvent.Add(_iTargetID, pEvent);
        }
    }


    public float GetProgress(int _iTargetID)
    {
        if (m_hashQuestEvent.ContainsKey(_iTargetID) == true)
        {
            ProgressEvent pEvent = m_hashQuestEvent[_iTargetID];
            return pEvent.CurrentAmount / pEvent.Amount;
        }
        return 0.0f;
    }

    public bool UpdateProgress(int _iTargetID, int _iAmount)
    {
        if (m_hashQuestEvent.TryGetValue(_iTargetID, out var Value) == true)
        {
            Value.CurrentAmount += _iAmount;
            if(Value.CurrentAmount >= Value.Amount)
            {
                //보상 UI 활성화
                return true;
            }
        }
        return false;
    }

}
