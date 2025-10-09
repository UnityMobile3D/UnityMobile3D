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

        [Header("Excution")]
        [SerializeField]        private ShapeProfile Shape;
        [SerializeField]        private CooldownProfile cooldown;
        [SerializeField]        private OriginProfile origin;

        [Header("Display")]
        [SerializeField]        private PresentationProfile presentation;
    }


    #region Meta Profiles
    [CreateAssetMenu(menuName = "SO/Profiles/Meta", fileName = "MetaProfile")]
    public class MetaProfile : ScriptableObject
    {
        public string skillName;          // 스킬 이름
        [TextArea] public string description;        // 스킬 설명
        public Sprite icon;               // 스킬 아이콘
        public Game.Common.SkillType type;               // 스킬 유형 (액티브/패시브)
        public Game.Common.SkillTag tags;               // 스킬 태그 (공격/버프/디버프/회복/군중제어/이동)
        public int levelRequirement;   // 레벨 요구사항
        public Game.Common.Element elementAffinity;    // 속성 (불, 물, 바람, 땅, 빛, 어둠)
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
        public DamageType damageType;          // 데미지 유형 (물리/마법/고정)

        [Header("Type")]
        public Game.Common.Element element;    // 속성 (불, 물, 바람, 땅, 빛, 어둠)
        public float lifesteal;                // 생명력 흡수
    }
    #endregion

    #region Execution Profiles
    [CreateAssetMenu(menuName = "SO/Profiles/Execution", fileName = "ShapeProfile")]
    public class ShapeProfile : ScriptableObject
    {
        public Game.Common.AreaShape shape;         // 스킬 범위 형태 (원/박스/콘/투사체)
        [Min(0)] public float radius;               // 반지름 (원/콘)
        [Min(0)] public float length;               // 길이 (박스/선)
        [Min(0)] public float width;                // 너비 (박스/선)
        [Range(0f, 360f)] public float angle;       // 각도 (콘)

#if UNITY_EDITOR    // 에디터에서만 유효성 검사
        private void OnValidate()
        {
            // shape에 따라 유효성 검사
            switch (shape)
            {
                case Game.Common.AreaShape.None:
                    radius = 0;
                    length = 0;
                    width = 0;
                    angle = 0;
                    break;
                case Game.Common.AreaShape.Circle:
                    length = 0;
                    width = 0;
                    angle = 0;
                    break;
                case Game.Common.AreaShape.Box:
                    radius = 0;
                    angle = 0;
                    break;
                case Game.Common.AreaShape.Cone:
                    length = 0;
                    width = 0;
                    break;
                case Game.Common.AreaShape.Line:
                    radius = 0;
                    angle = 0;
                    break;
                default:
                    break;
            }
        }
#endif
    }

    [CreateAssetMenu(menuName = "SO/Profiles/Execution", fileName = "TargetingProfile")]
    public class TargetingProfile : ScriptableObject
    {
        [Min(0f)] public int MaxTargets = 1;        // 최대 대상 수 (0 = 무제한)
        public LayerMask TargetLayers;              // 대상 레이어
        public Game.Common.TargetType targetType;   // 대상 유형 (자신/적군/지역/모두)
    }

    [CreateAssetMenu(menuName = "SO/Profiles/Execution", fileName = "CastProfile")]
    public class CastProfile : ScriptableObject
    {
        public Game.Common.CastType castType;       // 시전 유형 (즉시,채널링,차징,시전,토글)

        // 공통 페이즈 시간
        [Min(0f)] public float windupTime;         // 준비 시간 : 선딜
        [Min(0f)] public float recoveryTime;       // 회복 시간 : 후딜

        // 타입별 추가 시간
        [Min(0f)] public float channelDuration;         // 채널링 지속 시간 (채널링 전용)
        [Min(0f)] public float tickInterval;            // 채널링 틱 간격 (채널링/토글 전용)
        [Min(0f)] public float maxChargeTime;           // 최대 차징 시간 (차징 전용)
        [Range(0f,1f)] public float minChargePercent;   // 최소 차징 비율 (차징 전용)

        // 인터럽트/슈퍼아머/무적
        public bool interruptible = true;           // 피격 등으로 끊김 허용
        public bool superArmorDuringCast = false;   // 시전 중 슈퍼아머
        public bool invincibleDuringCast = false;   // 시전 중 무적
        public float invincibleStart = 0f;          // 무적 시작 시간 (시전 시작 후)
        public float invincibleEnd = 0f;            // 무적 종료 시간 (시전 종료 전)

        // 이동
        public bool canMoveDuringWindup = false;    // 준비 시간 동안 이동 가능
        public bool canMoveDuringChannel = false;   // 채널링 시간 동안 이동 가능
        public bool canMoveDuringRecovery = false;  // 회복 시간 동안 이동 가능
        public bool canMoveDuringCharge = false;    // 차징 시간 동안 이동 가능
        
        [Range(0f, 1f)] public float moveSpeedMultiplierDuringCast = 1f;      // 시전 중 이동 속도 배율
        
        public bool canRotateDuringCast = true;    // 시전 중 회전 가능
        public bool canRotateDuringChannel = false; // 채널링 중 회전 가능
        public bool canRotateDuringRecovery = false;    // 회복 중 회전 가능
        public bool canRotateDuringCharge = true;  // 차징 중 회전 가능

        public bool faceTargetDuringCast = false;  // 시전 중 대상 바라보기
        public float facingLockSeconds = 0f;       // 시전 후 바라보기 고정 시간 (0 = 즉시 해제)
    }

    [CreateAssetMenu(menuName = "SO/Profiles/Execution", fileName = "CooldownProfile")]
    public class CooldownProfile : ScriptableObject
    {
        [Min(0f)] public float baseCooldown;        // 기본 쿨타임
        [Min(0)] public int charges;             // 스킬 충전 수
        [Min(0f)] public float chargeRegenTime;     // 충전 재생 시간

        public CooldownType cooldownType;           // 쿨타임 타입 (고정/가변)
        public string sharedGroupId;                // 공유 쿨타임 그룹 ID
        public bool usesGCD;                        // GCD 사용 여부
    }

    [CreateAssetMenu(menuName = "SO/Profiles/Execution", fileName = "OriginProfile")]
    public class OriginProfile : ScriptableObject
    {
        public Origin origin;                       // 스킬 범위 기준점
        public Transform customOrigin;              // 커스텀 기준점 (없으면 null)
        public bool isProjectile;                   // 투사체 여부
        public float projectileSpeed;               // 투사체 속도 (투사체일 경우)
        public float projectileRange;               // 투사체 사거리 (투사체일 경우)
        public bool followTarget;                   // 대상 추적 여부 (투사체일 경우)
    }

#endregion

    #region Presentation Profiles
    [CreateAssetMenu(menuName = "SO/Profiles/Presentation", fileName = "PresentationProfile")]
    public class PresentationProfile : ScriptableObject
    {
        [Header("Animation")]
        public AnimationClip castAnim;     // 시전 애니메이션

        [Header("VFX/SFX")]
        public GameObject vfxCast;         // 시전 VFX
        public GameObject vfxImpact;       // 타격 VFX
        public AudioClip sfxCast;          // 시전 SFX
        public AudioClip sfxHit;           // 타격 SFX

        [Header("Feedback")]
        public bool     cameraShakeAmplitude; // 카메라 흔들림
        public float    cameraShakeDuration;  // 카메라 흔들림 지속 시간
        public bool     hitPause;              // 타격 시 일시정지

        public UICooldownStyle uiCooldownStyle; // 쿨타임 UI 스타일
        public TrailRenderer trailProfile; // 투사체 궤적 프로필
    }
    #endregion
}