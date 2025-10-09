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
        public string strType;     // "tap","stay","swipe","drag","pinch","rotate" �� �̺�Ʈ Ÿ��
        public Vector2 vDelta;     // ������ �̵���(�巡��/�������� ��)
        public float fValue;       // ��ġ ������ ��ȭ/ȸ�� ����/�������� ���ӽð� �� �߰� ��
        public bool bOverUI;       // ���� �� UI ��������(��å: ���� UI�� ������ UI)
    }

    public static InputManager m_Instance = null;
    private UIRayCaster m_pUIRayCaster;
    private TouchTracker m_pTouchTracker;
    private ActionMapper m_pActionMapper;

    private readonly List<tTouchEvent> m_listTouchEvent = new(10);//������ ����

    private readonly ActionState m_pActionState = new ActionState(); //�ʵ忡 �ٸ� �ν��Ͻ��� �ٽ� ���� �� �Ѵ١�

    public ActionState State => m_pActionState; //�ܺ� ������ eadonly �ʵ� + get-only ������Ƽ
    //Test
    private InputAction m_pMoveAction;
    
    [SerializeField] private EventSystem m_pEventSystem;             // UGUI EventSystem ����
    [SerializeField] private List<GraphicRaycaster> m_pUIRay;        // ��ȣ�ۿ� Canvas�� GraphicRaycaster ���

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
