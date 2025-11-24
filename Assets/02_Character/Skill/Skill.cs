using Game.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eActionID = InputManager.eActionID;

public class Skill : MonoBehaviour
{
    //SO에서 설정한 속성값 셋팅 후 런타임에서 관리
    [Serializable]
    public class SkillContext
    {
        public Skill skill;                            //SO를 들고있는 skill Comonent
        public float time;                             //누적 시간

        public eActionID eActionID = eActionID.None; //스킬 사용시 맵핑된 액션 아이디
    }
    //기본 스킬 틀에서 차지 스킬, 즉발, 캐스팅 

    private Player m_pPlayer = null;
    public Player OwnerPlayer => m_pPlayer;

    [SerializeField] private SOSKill m_pSkill = null;
    public SOSKill CurrentSkill => m_pSkill;

    [SerializeField] private SkillContext m_pSkillContext = new SkillContext();

    private int m_iCurSkillIdx = 0;

    public void Init(SOSKill _pSKill, eActionID _eBindActionID)
    {
        m_iCurSkillIdx = 0;
        m_pSkill = _pSKill;

        m_pSkillContext.eActionID = _eBindActionID;
    }
  
    public void StartSkill()  //스킬이 시작
    {
        m_pSkill.Loggic.startSkillLogic?.UpdateSkill(m_pSkillContext);
    }
    public void EndSkill()    //스킬이 끝나면
    {
        m_pSkill.Loggic.endSkillLogic?.UpdateSkill(m_pSkillContext);

        m_pSkill = null;
    }

    public bool UpdateSkill() //스킬이 진행되는 동안
    {
        List<SOSkillLogic> listSkill = m_pSkill.Loggic.skillLogic;
        if(listSkill[m_iCurSkillIdx].UpdateSkill(m_pSkillContext) == true)
            ++m_iCurSkillIdx;
        
        
        if(m_iCurSkillIdx > listSkill.Count -1)
        {
            m_iCurSkillIdx = 0; //초기화
            return true;
        }

        return false;
    }
}
