using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using PointerInputState = ActionMapper.PointerInputState;
using ActionState = ActionMapper.ActionState;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEditor.PackageManager;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using System;


public class InputManager : MonoBehaviour
{
    //키보드 값과 맵핑하기 위해서 
    [System.Serializable]
    public enum eActionID
    {
        None,
        //Boolean
        SkillDefault,
        Skill1,
        Skill2,
        SubSkill,
        Item,

        //Vector2D
        Move,

        End,
    }

    [System.Serializable]
    public class ActionBinding
    {
        public eActionID ID = eActionID.None;
        public PED InputFunction = null;
        public List<InputActionReference> listAction = new(); // <- 인스펙터에서 선택 가능
    }

    public struct tTouchEvent
    {
        public string strType;     // "tap","stay","swipe","drag","pinch","rotate" 등 이벤트 타입
        public Vector2 vDelta;     // 프레임 이동량(드래그/스와이프 등)
        public float fValue;       // 핀치 스케일 변화/회전 각도/롱프레스 지속시간 등 추가 값
        public bool bOverUI;       // 시작 시 UI 위였는지(정책: 시작 UI면 끝까지 UI)
    }

    public static InputManager m_Instance = null;
    private UIRayCaster m_pUIRayCaster;
    private TouchTracker m_pTouchTracker;
    private ActionMapper m_pActionMapper;

   
    [SerializeField] private List<ActionBinding> m_listActions = new();

    private readonly List<tTouchEvent> m_listTouchEvent = new(10);//프레임 버퍼
    private readonly PointerInputState m_pPointerState = new PointerInputState(); 
    private readonly ActionState m_pActionState = new ActionState();

    //UGUI 이벤트 시스템과 내 InputManager에 데이터를 동기화하기 위한 임시 데이터
    private ActionState m_pUGUIActionState = new ActionState();
    
    public PointerInputState PointerState => m_pPointerState; //외부 참조용 eadonly 필드 + get-only 프로퍼티
    public ActionState ActionState => m_pActionState;
    
    [SerializeField] private EventSystem m_pEventSystem;             // UGUI EventSystem 참조
    [SerializeField] private List<GraphicRaycaster> m_pUIRay;        // 상호작용 Canvas의 GraphicRaycaster 목록

    private void Awake()
    {
        if (m_Instance != null)
            Destroy(this);

        m_Instance = this;

        m_pUIRayCaster = new UIRayCaster(m_pEventSystem, m_pUIRay);
        m_pTouchTracker = new TouchTracker();
        m_pActionMapper = new ActionMapper();


        for(int i = 0; i<m_listActions.Count; ++i)
        {
            var pActionRef = m_listActions[i];
            if (pActionRef == null)
                continue;

            for(int j = 0; j< pActionRef.listAction.Count; ++j)
            {
                pActionRef.listAction[j].action.Enable();
            }
        }
    }

    private void Update()
    { 
        //패드랑 관련 없는 바탕, 그래드 관련된 액션들
        var listActiveTouch = Touch.activeTouches;
        if (listActiveTouch.Count> 0)
        {
            m_pTouchTracker.UpdateTouchPhase(ref listActiveTouch, m_listTouchEvent, m_pUIRayCaster);
            m_pActionMapper.MapPointer(m_listTouchEvent, m_pPointerState);
        }

        //UGUI와 연동된 액션
        m_pActionState.Clear();

        m_pActionState.CopyFrom(m_pUGUIActionState);

        //UGUI나 다른 다바이스 기기와 연동된 캐릭터 인풋 관련된 액션들
        m_pActionMapper.MapDevice(m_pActionState, m_listActions);

        m_pUGUIActionState.Clear();



    }

    private void LateUpdate()
    {

        //디바이스 전체를 순회해서 
    }

    public void BindUGUIButtonBoolean(eActionID _eID, bool _bValue)
    {
        m_pUGUIActionState.SetBoolean(_eID, _bValue);
    }

    public void BindUGUIButtonVector2D(eActionID _eID, in Vector2 _vValue)
    {
        m_pUGUIActionState.SetVector2D(_eID, _vValue);
    }

}
