using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using tTouchEvent = InputManager.tTouchEvent;
public class ActionMapper 
{
    public sealed class ActionState
    {
        public Vector2 vMove;      // 이동 벡터(정규화/클램프)
        public Vector2 vSwipeDelta; // 스와이프 벡터(프레임 이동량)

        public float fZoomDelta;   // 핀치로 인한 줌 변화량 누적
        public float fRotateDelta; // 두 손가락 회전 각 변화량 누적(도 단위)

        public bool bFireDown;     // 눌림 시작 트리거(한 프레임)
        public bool bFireHeld;     // (연속)
        public bool bFireUp;       // 해제 트리거(한 프레임)

        public ActionState(){}
       
        public void ClearAction() { bFireDown = bFireUp = false; fZoomDelta = 0; fRotateDelta = 0; }
    }

    private Rect m_tLeft;
    private Rect m_tRight;

    private void calculate_rect()
    {
        m_tLeft = new Rect(0, 0, Screen.width * 0.45f, Screen.height);                      // 왼쪽 45%: 이동
        m_tRight = new Rect(Screen.width * 0.55f, 0, Screen.width * 0.45f, Screen.height);  //오른쪽 45%: 에임/사격
    }


    public void Map(List<tTouchEvent> _listTouchEvent, ActionState _pState)
    {
        //if (m_tLeft.width + m_tRight.width < Screen.width * 0.9f)
        //    calculate_rect();

        _pState.ClearAction();

        foreach(var tEve in _listTouchEvent)
        {
            if (tEve.bOverUI)
                continue;

            switch(tEve.strType)
            {
                case "drag":
                    //if(m_tLeft.Contains(tEve.vPos))
                    //_pState.vMove += tEve.vDelta / 100.0f;  // 왼쪽: 이동 벡터
                    _pState.vMove = Vector2.ClampMagnitude(_pState.vMove, 1.0f); // 이동 벡터 정규화
                    break;

                case "swipe":
                    _pState.vSwipeDelta += Vector2.ClampMagnitude(_pState.vMove, 1.0f); // 스와이프 벡터 누적
                    break;

                case "tap":
                    //if (m_tLeft.Contains(tEve.vPos))
                     _pState.bFireDown = true;  
                    break;

                case "stay":
                    //if (m_tRight.Contains(tEve.vPos))
                     _pState.bFireHeld = true;  // 롱프레스 유지
                    break;

                case "pinch":
                    _pState.fZoomDelta += tEve.fValue; // 두 손가락 회전 각도 변화량
                    break;

                case "rotate":
                    _pState.fRotateDelta += tEve.fValue; // 두 손가락 회전 각도 변화량
                    break;

            }
        }
    }
}

