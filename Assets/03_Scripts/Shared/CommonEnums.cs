using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{
    // =========================================================
    //                         범위 관련
    // =========================================================

    public enum AreaShape : int
    {
        None    = 0,    // 없음
        Circle  = 1,    // 원
        Box     = 2,    // 박스
        Cone    = 3,    // 콘
        Line    = 4     // 선
    }

    public enum Origin  // 스킬 범위 기준점
    {
        Self,           // 시전자
        TargetedPoint,  // 대상 지점
        TargetedEntity  // 대상 유닛
    }


    // =========================================================
    //                         스킬 관련
    // =========================================================

    public enum SkillType : int       // 스킬 유형
    {
        Active,         // 액티브
        Passive         // 패시브
    }

    public enum SkillTag : int
    {
        None = 0,        // 없음
        Attack = 1 << 0,   // 1  : 공격
        Buff = 1 << 1,   // 2  : 버프
        Debuff = 1 << 2,   // 4  : 디버프
        Heal = 1 << 3,   // 8  : 회복
        CrowdControl = 1 << 4,   // 16 : 군중제어
        Movement = 1 << 5    // 32 : 이동
    }
    public enum CooldownType           // 쿨타임 타입 (고정/가변)
    {
        Fixed,
        Variable
    }

    public enum UICooldownStyle       // 쿨타임 UI 스타일
    {
        Radial, // 원형
        Linear, // 직선
        None
    }


    public enum Element : int
    {
        None = 0,    // 없음
        Fire = 1,    // 불
        Water = 2,    // 물
        Wind = 3,    // 바람
        Earth = 4,    // 땅
        Light = 5,    // 빛
        Dark = 6     // 어둠
    }

    public enum ElementMask : int
    {
        None = 0,        // 없음
        Fire = 1 << 0,   // 1  : 불
        Water = 1 << 1,   // 2  : 물
        Wind = 1 << 2,   // 4  : 바람
        Earth = 1 << 3,   // 8  : 땅
        Light = 1 << 4,   // 16 : 빛
        Dark = 1 << 5,   // 32 : 어둠
        All = ~0        // 전체
    }
    

    public enum TargetType   // 대상 타입
    {
        Slef,   // 자신
        Enemy,  // 적군
        Ground, // 지점
        All,    // 전체
    }

    public enum CastType     // 시전 타입
    {
        Instant,    // 즉시   : 즉발
        Channeled,  // 채널링  : 스킬 발동과 동시에 효과적용, 그러나 스킬을 유지하고있어야 효과 지속, 시전 중 동작 불가
        Charged,    // 차징   : 스킬 게이지를 모아서 발동, 최대치에 도달하면 자동 발동, 중간에 취소 가능.
        Cast,       // 시전   : 스킬 시전 준비 시간 필요함, 캐스팅 도중 동작 불가
        Toggle      // 토글   : 스킬 켜고 끄기 가능, 켜져있는 동안 지속 효과, 켜고 끌 때 동작 불가
    }


    // =========================================================
    //                      데미지 관련
    // =========================================================

    public enum DamageType        // 데미지 유형 (물리/마법/고정)
    {
        Physical,
        Magical,
        True
    }

}