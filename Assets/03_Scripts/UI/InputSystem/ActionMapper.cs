using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using tTouchEvent = InputManager.tTouchEvent;
public class ActionMapper 
{
    public sealed class ActionState
    {
        public Vector2 vMove;      // �̵� ����(����ȭ/Ŭ����)
        public Vector2 vSwipeDelta; // �������� ����(������ �̵���)

        public float fZoomDelta;   // ��ġ�� ���� �� ��ȭ�� ����
        public float fRotateDelta; // �� �հ��� ȸ�� �� ��ȭ�� ����(�� ����)

        public bool bFireDown;     // ���� ���� Ʈ����(�� ������)
        public bool bFireHeld;     // (����)
        public bool bFireUp;       // ���� Ʈ����(�� ������)

        public ActionState(){}
       
        public void ClearAction() { bFireDown = bFireUp = false; fZoomDelta = 0; fRotateDelta = 0; }
    }

    private Rect m_tLeft;
    private Rect m_tRight;

    private void calculate_rect()
    {
        m_tLeft = new Rect(0, 0, Screen.width * 0.45f, Screen.height);                      // ���� 45%: �̵�
        m_tRight = new Rect(Screen.width * 0.55f, 0, Screen.width * 0.45f, Screen.height);  //������ 45%: ����/���
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
                    //_pState.vMove += tEve.vDelta / 100.0f;  // ����: �̵� ����
                    _pState.vMove = Vector2.ClampMagnitude(_pState.vMove, 1.0f); // �̵� ���� ����ȭ
                    break;

                case "swipe":
                    _pState.vSwipeDelta += Vector2.ClampMagnitude(_pState.vMove, 1.0f); // �������� ���� ����
                    break;

                case "tap":
                    //if (m_tLeft.Contains(tEve.vPos))
                     _pState.bFireDown = true;  
                    break;

                case "stay":
                    //if (m_tRight.Contains(tEve.vPos))
                     _pState.bFireHeld = true;  // �������� ����
                    break;

                case "pinch":
                    _pState.fZoomDelta += tEve.fValue; // �� �հ��� ȸ�� ���� ��ȭ��
                    break;

                case "rotate":
                    _pState.fRotateDelta += tEve.fValue; // �� �հ��� ȸ�� ���� ��ȭ��
                    break;

            }
        }
    }
}

