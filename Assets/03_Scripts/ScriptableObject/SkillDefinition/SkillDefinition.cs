using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Game.Common;
using Game.Skill;

namespace Game.Skill
{
    [CreateAssetMenu(fileName = "SkillDefinition", menuName = "SO/Skill", order = int.MaxValue)]
    public class SOSKill : ScriptableObject
    {
        [Header("Meta")]
        [SerializeField]        private MetaProfile meta;

        [Header("Damage")]
        [SerializeField]        private DamageProfile damage;

        [Header("Option")]
        [SerializeField]        private SkillOptionProfile option;

        [Header("Logic")]
        [SerializeField]        private LogicProfile logic;
        public MetaProfile Meta => meta;
        public DamageProfile Damage => damage;

        public SkillOptionProfile Option => option;

        public LogicProfile Loggic => logic;
    }

    #region Meta Profiles
    [CreateAssetMenu(menuName = "SO/Profiles/Meta", fileName = "MetaProfile")]
    public class MetaProfile : ScriptableObject
    {
        public int skillid;                          // 스킬 ID
        public string skillName;                     // 스킬 이름
        [TextArea] public string description;        // 스킬 설명  
        public int levelRequirement;                 // 레벨 요구사항
    }
    #endregion
    #region Damage Profiles
    [CreateAssetMenu(menuName = "SO/Profiles/Damage", fileName = "DamageProfile")]
    public class DamageProfile : ScriptableObject
    {
        [Header("Base")]
        public float baseDamage;               // 기본 데미지
        public float coefficient;              // 계수
        public float criticalChance;           // 치명타 확률
        public float criticalMultiplier;       // 치명타 배율
        public float damageVariance;           // 데미지 변동폭

    }
    #endregion


    #region Skill Option
    [CreateAssetMenu(menuName = "SO/Profiles/SkillOption", fileName = "SkillOptionProfile")]
    public class SkillOptionProfile : ScriptableObject
    {
        public float cooldown   = 1.0f;                    //쿨타임
        public float manacost   = 1.0f;                    //마나 소모량
        public float power      = 1.0f;                    //스킬 파워
        public float chargetime = 0.0f;                    //차지 시간

        public float lifeTime;                             //데미지 오브젝트 생존 시간
        public float moveSpeed;                            //이동 속도


        public CastType castType;                          //시전 타입
        public TargetingProfile targetingProfile;          //타겟팅 프로필
    }
    #endregion


    [CreateAssetMenu(menuName = "SO/Profiles/Execution", fileName = "TargetingProfile")]
    public class TargetingProfile : ScriptableObject
    {
        [Min(0f)] public int MaxTargets = 1;        // 최대 대상 수 (0 = 무제한)
        public LayerMask TargetLayers;              // 대상 레이어
        public Game.Common.TargetType targetType;   // 대상 유형 (자신/적군/지역/모두)


        public bool    targetPosition = false;      //타겟 위치로
        public bool    playerFacing = false;        // 플레이어가 바라보는 방향으로 스킬 발동
        public Vector3 offset = Vector3.zero;       // 생성될 위치 오프셋
        public float   range = 0f;                  // 스킬 사거리
        public float   spawnDistance = 0f;          // 스킬이 생성될 거리
    }

    #region Logic Profiles

    [CreateAssetMenu(menuName = "SO/Profiles/Logic", fileName = "LogicProfile")]
    public class LogicProfile : ScriptableObject
    {
        public List<SOSkillLogic> skillLogic;
        public SOSkillLogic endSkillLogic;
        public SOSkillLogic startSkillLogic;
    }

    #endregion

}