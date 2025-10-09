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

    public enum SkillSlot : int { DefaultAttack = 0, Skill1 = 1, Skill2 = 2, Count = 3 } // 스킬 슬롯
    public SkillDefinition[] _skills = new SkillDefinition[(int)SkillSlot.Count]; // 스킬 슬롯 (UI버튼)

    // 스킬 자원 관리.
    // 캐릭터 오브젝트에서 받아올 것,
    // 
    

    public void SetSkillDefinition(SkillSlot slot, SkillDefinition skill)
    {
        _skills[(int)slot] = skill;
    }

    // 몬스터는 생성시 스킬 우선 등록, 플레이어는 수시로 변경 가능하도록.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void UseSkill()
    {
        // 인자는 사용 할 스킬 슬롯,

        // UI를 누르면, Player의 Attack 함수 호출.
        // Attack은 연결된 스킬사용을 호출. 인자로 스킬러너에 디스크립션을 관리
    }
}
