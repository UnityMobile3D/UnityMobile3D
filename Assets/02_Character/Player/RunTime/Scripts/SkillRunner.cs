using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Skill;

public class SkillRunner : MonoBehaviour
{
    // Player에 등록된 SO를 불러와서 스킬을 실행하는 컴포넌트
    // 추후에 스킬 트리나 쿨타임 관리 등도 여기서 담당할 수 있음
    // 현재는 기본 구조만 구현
    // 추후에 필요에 따라 기능 추가 가능

    public enum eSkillType
    {
        None,
        Default,
        SubSkill,
        MainSkill,
    }
    public SOSKill[] _skills = new SOSKill[(int)eSkillType.MainSkill + 1];  // 스킬 슬롯 (UI버튼)
    private SOSKill _runSkill = null;                                       // 현재 실행중인 스킬
    public SOSKill RunSkill => _runSkill;
    // 스킬 자원 관리.
    // 캐릭터 오브젝트에서 받아올 것,
    // 
    
    public void SetSkillDefinition(eSkillType _eSkillType, SOSKill skill)
    {
        _skills[(int)_eSkillType] = skill;
    }

    // 몬스터는 생성시 스킬 우선 등록, 플레이어는 수시로 변경 가능하도록.

    public void UseSkill(eSkillType _eSkillType)
    {
        // 인자는 사용 할 스킬 슬롯,

        // UI를 누르면, Player의 Attack 함수 호출.
        // Attack은 연결된 스킬사용을 호출. 인자로 스킬러너에 디스크립션을 관리

        if(_runSkill != null)
            return; // 이미 스킬 실행중

        _runSkill = _skills[(int)_eSkillType];

        if (_runSkill == null)
            return;
    }

    //현재 활성화 된 스킬 실행
    public void UpdateSkill()
    {

    }

}
