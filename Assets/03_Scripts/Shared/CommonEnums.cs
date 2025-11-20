using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{
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

  

}