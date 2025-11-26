using Game.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eActionID = InputManager.eActionID;

//만약 프레임이 중요한 스킬이라면 Logic에서 해당 프레임까지 기다리는 로직 생성
public class SkillRunner : MonoBehaviour
{
    //SO에서 설정한 속성값 셋팅 후 런타임에서 관리
    [Serializable]
    public class SkillContext
    {
        public SkillRunner skill;                                         //SO를 들고있는 skill Comonent
        public Animator animator;                                             
        public GameObject runSkillObject = null;                          //소환된 오브젝트

        public bool pressed = false;                                       //스킬에 해당하는 버튼 누름 여부
        public float chargeTime;                                           //누적 시간

        public List<IChargeEvent> chargeEvents = new List<IChargeEvent>(); //차지 이벤트 (스킬 오브젝트의 리스트 참조기 때문에 Clear X

        public bool hitEvent = false;                                      //히트 이벤트 발생 여부
        public int hitIndex = -1;                                          //히트 이벤트 프레임
        public int animHashName;                                         //현재 애니메이션 해시 이름  
    }
    [Serializable]
    public enum eSkillType
    {
        None,
        Default,
        SubSkill,
        MainSkill,
    }

    private CoolDownView[] _cooldowns = null;
    private CoolDownView _runCollDown = null;

    private SOSKill[] _skills;
    private SOSKill _runSkill = null;                                       // 현재 실행중인 스킬
    public SOSKill RunSkill => _runSkill;
    //기본 스킬 틀에서 차지 스킬, 즉발, 캐스팅 

    private Player _player = null;
    public Player OwnerPlayer => _player;

    [SerializeField] private SkillContext _skillContext = new SkillContext();
    public SkillContext Context => _skillContext;

    private int _iCurrentSkillIdx = 0;

    public void Awake()
    {
        _player = GetComponent<Player>();
        _skillContext.animator = _player.Animator;
        _skillContext.skill = this;
        
    }
    public void Start()
    {
        _skills = new SOSKill[PlayerInterfaceSlot.InterfaceSlotCount];
        _cooldowns = new CoolDownView[PlayerInterfaceSlot.InterfaceSlotCount];
    }

    public void Update()
    {
        for(int i = 0; i<_cooldowns.Length; ++i)
        {
            if (_cooldowns[i] == null || _cooldowns[i].IsDone == true)
                continue;

            _cooldowns[i].UpdateCoolTime(Time.deltaTime);
        }

    }
    public void SetSkillDefinition(int slotIdx, SOSKill skill, CoolDownView cooldown)
    {
        _skills[slotIdx] = skill;
        _cooldowns[slotIdx] = cooldown;
    }

    // 몬스터는 생성시 스킬 우선 등록, 플레이어는 수시로 변경 가능하도록.

    public void UseSkill(int slotIdx , CoolDownView cooldown)
    {
        // 인자는 사용 할 스킬 슬롯,

        // UI를 누르면, Player의 Attack 함수 호출.
        // Attack은 연결된 스킬사용을 호출. 인자로 스킬러너에 디스크립션을 관리
     
        // 이미 스킬 실행중, 이미 누르고 있는 스킬이 아니라면
        if (_runSkill == _skills[slotIdx] || _skills[slotIdx] == null)
            return;

        // 쿨타임 체크
        if (_cooldowns[slotIdx].IsDone == false)
            return;

        _runSkill = _skills[slotIdx];
        _runCollDown = cooldown;
        _iCurrentSkillIdx = 0;

        StartSkill();
    }

    public void StartSkill()  //스킬이 시작
    {
        //애니메이션 시작
        
        _runSkill.Loggic.startSkillLogic?.UpdateSkill(_skillContext);
        _skillContext.pressed = true;

        //애니메이션 실행
        AnimationSetting();
        
    }
    public void EndSkill()    //스킬이 끝나면
    {
        _runSkill.Loggic.endSkillLogic?.UpdateSkill(_skillContext);

        OffSkill();
    }

    public void OffSkill()          //초기화
    {
        _runCollDown.ResetCoolTime();

        _iCurrentSkillIdx = 0; 
        _runSkill = null;
        _runCollDown = null;

        _skillContext.chargeTime = 0.0f;
        _skillContext.pressed = false;
        _skillContext.runSkillObject = null;

        _skillContext.hitEvent = false;
        _skillContext.hitIndex = -1;

    }


    public void EndPressedSkill()       
    {
        _skillContext.pressed = false;
    }

    public void OnSkillHitEvent(int hitEvent)
    {
        _skillContext.hitEvent = true;
    }

    public bool UpdateSkill() //스킬이 진행되는 동안
    {
        List<SOSkillLogic> listSkill = _runSkill.Loggic.skillLogic;
        if(listSkill[_iCurrentSkillIdx].UpdateSkill(_skillContext) == true)
            ++_iCurrentSkillIdx;

        if(_iCurrentSkillIdx > listSkill.Count -1)
        {
            EndSkill();
            return true;
        }

        return false;
    }

    private void AnimationSetting()
    {
        AnimatorStateInfo animState =
                       _player.Animator.GetCurrentAnimatorStateInfo(_runSkill.Animation.layerIndex);

        switch (_runSkill.Animation.playMode)
        {
            case SkillAnimPlayMode.Trigger:
                {
                    _player.Animator.SetTrigger(_runSkill.Animation.triggerName);
                }
                break;

            case SkillAnimPlayMode.CrossFadeState:
                { 
                    // 같은 상태인데 다시 재생 원치 않으면 skip
                    if (animState.IsName(_runSkill.Animation.stateName) == true)
                        return;

                    _player.Animator.CrossFade(_runSkill.Animation.stateName,
                        _runSkill.Animation.crossFadeTime, _runSkill.Animation.layerIndex, 0f);
                }
                break;
        }

        _skillContext.animHashName = Animator.StringToHash(_runSkill.Animation.stateName);
    }
}
