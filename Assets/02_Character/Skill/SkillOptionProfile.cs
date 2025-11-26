using Game.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Profiles/SkillOption", fileName = "SkillOptionProfile")]
public class SkillOptionProfile : ScriptableObject
{
    public float cooldown = 1.0f;                    //쿨타임
    public float manacost = 1.0f;                    //마나 소모량
    public float power = 1.0f;                    //스킬 파워
    public float chargetime = 0.0f;                    //차지 시간

    public float lifeTime;                             //데미지 오브젝트 생존 시간
    public float moveSpeed;                            //이동 속도


    public CastType castType;                          //시전 타입
    public TargetingProfile targetingProfile;          //타겟팅 프로필
}