using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Game.Common;
using Game.Skill;

namespace Game.Skill
{
    [CreateAssetMenu(fileName = "SkillDefinition", menuName = "SO/Skill", order = int.MaxValue)]
    public class SkillDefinition : ScriptableObject
    {
        [Header("Meta")]
        [SerializeField]        private MetaProfile meta;

        [Header("Logic")]
        [SerializeField]        private DamageProfile damage;
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
    #region Logic Profiles
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


    [CreateAssetMenu(menuName = "SO/Profiles/Execution", fileName = "TargetingProfile")]
    public class TargetingProfile : ScriptableObject
    {
        [Min(0f)] public int MaxTargets = 1;        // 최대 대상 수 (0 = 무제한)
        public LayerMask TargetLayers;              // 대상 레이어
        public Game.Common.TargetType targetType;   // 대상 유형 (자신/적군/지역/모두)
    }

}