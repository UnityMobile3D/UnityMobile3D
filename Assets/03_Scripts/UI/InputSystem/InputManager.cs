using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using ActionState = ActionMapper.ActionState;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEditor.PackageManager;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;
using UnityEngine.Assertions;


public class InputManager : MonoBehaviour
{
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

    private readonly List<tTouchEvent> m_listTouchEvent = new(10);//프레임 버퍼

    private readonly ActionState m_pActionState = new ActionState(); //필드에 다른 인스턴스를 다시 대입 못 한다”

    public ActionState State => m_pActionState; //외부 참조용 eadonly 필드 + get-only 프로퍼티
    //Test
    private InputAction m_pMoveAction;
    
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

       
        m_pMoveAction = new("Move", InputActionType.Value);
        m_pMoveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
        m_pMoveAction.AddBinding("<Gamepad>/leftStick");
        m_pMoveAction.Enable();


    }

    private void Update()
    { 
        var listActiveTouch = Touch.activeTouches;
        if (listActiveTouch.Count> 0)
        {
            m_pTouchTracker.UpdateTouchPhase(ref listActiveTouch, m_listTouchEvent, m_pUIRayCaster);
            m_pActionMapper.Map(m_listTouchEvent, m_pActionState);
        }
    }

  
}
