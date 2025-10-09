using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using InputFrame = InputManager.InputFrame;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using tTouchEvent = InputManager.tTouchEvent;

using System.Linq;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Utilities;
using UnityEditor.Playables;

public class TouchTracker 
{

    //private Dictionary<int, tTrack> m_hashTrack = new Dictionary<int, tTrack>();
    List<bool> m_listTouchOverUI = Enumerable.Repeat(false, 10).ToList();

    private const float m_fTapMaxTime = 0.2f;  // �� �ִ� ���ӽð�
    private const float m_fTapMaxMove = 3.0f;   // �� ��� �̵���
    private const float m_fLongPressTime = 0.45f; // �������� �ּ� �ð�
    private const float m_fSwipeMinDist = 5.0f;   // �������� �ּ� �Ÿ�
    private const float m_fDragStartDist = 1.0f;   // �巡�� ���� �Ÿ�


    public void UpdateTouchPhase(ref ReadOnlyArray<Touch> _listTouch,
        List<tTouchEvent> _listResultTouch, UIRayCaster _pRayCaster)
    {
        _listResultTouch.Clear();

        foreach (var tTouch in _listTouch)
        {
            switch(tTouch.phase)
            {
                case TouchPhase.Began:
                    began_touch(tTouch, _pRayCaster);
                    break;
    
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    moved_touch(in tTouch, _listResultTouch);
                    break;
    
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    ended_touch(in tTouch, _listResultTouch);
                    break;
            }

        }

        //�� �ٲ��� �ʰ� ���� ó�� 2���� �켱���� �����ؼ�
        if (_listTouch.Count >= 2)
        {
            int iFirstID = 0;
            int iSecondID = 0;

            for(int i = 0; i< _listTouch.Count; ++i)
            {
                iFirstID = _listTouch[i].startTime > _listTouch[iFirstID].startTime ? i : iFirstID;
            }
            for(int i = 1; i< _listTouch.Count; ++i)
            {
                if (i != iFirstID)
                    iSecondID = _listTouch[i].startTime < _listTouch[iSecondID].startTime ? i : iSecondID;
            }
            track_multi_touch(_listResultTouch, _listTouch[iFirstID], _listTouch[iSecondID]);
        }
           
    }


    private void add_touch_event(List<tTouchEvent> _listTouchEvent, in string _strStateType, 
             in Vector2 _vDelta , float _fValue, bool _bOverUI, /*Test*/ int _iID)
    {
        _listTouchEvent.Add(new tTouchEvent
        {
            strType = _strStateType,
            vDelta = _vDelta,
            fValue = _fValue, 
            bOverUI = _bOverUI
        });

        Debug.Log($"ID {_iID} Ÿ�� {_strStateType} �� : {_fValue} " +
            $"�̵��� : {_vDelta} UI {_bOverUI}" );
    }

    private void track_multi_touch(List<tTouchEvent> _listTouchEvent, in Touch _tFirstTouch, in Touch _tSecondTouch)
    {
        int iFirstID = _tFirstTouch.finger.index;
        int iSecondID = _tSecondTouch.finger.index;

        //���� �ٲٱ� _tTouch�θ����ε� ��� Tack�� �Ⱦ��� List<bool> ������ overUi�� �˻�
        float fCurDist = (_tFirstTouch.screenPosition - _tSecondTouch.screenPosition).magnitude;                //���� �� �հ��� �Ÿ�
        float fStartDist = (_tFirstTouch.startScreenPosition - _tSecondTouch.startScreenPosition).magnitude;    //���� �� �հ��� �Ÿ�
        float fScale = fCurDist / fStartDist;                                                       //��ġ ������ ��ȭ

        add_touch_event(_listTouchEvent, "pinch", 
            /*(pFirst.vLastPos + pSecond.vLastPos) * 0.5f,*/ Vector2.zero, fScale,
            m_listTouchOverUI[iFirstID] && m_listTouchOverUI[iSecondID], iFirstID | iSecondID);

        //Vector2 vStartVector = pSecond.vStartPos - pFirst.vStartPos;        //���� �� �� �հ��� ��ġ�� ���� ����
        //Vector2 vCurVector = pSecond.vLastPos - pFirst.vLastPos;            //���� �� �հ��� ��ġ�� ���� ����
        //
        //
        //float fStartAngle = Mathf.Atan2(vStartVector.y, vStartVector.x) * Mathf.Rad2Deg; //���� ���� ���ȿ��� ��׸���
        //float fCurAngle = Mathf.Atan2(vCurVector.y, vCurVector.x) * Mathf.Rad2Deg; //���� ���� ���ȿ��� ��׸���
        //float fRotateDelta = Mathf.DeltaAngle(fStartAngle, fCurAngle); //ȸ�� ��ȭ��(�� ����)

        //_listTouchEvent.Add(new tTouchEvent
        //{
        //    strType = "rotate",
        //    iPointerIDA = pFirst.iID,
        //    iPointerIDB = pSecond.iID,
        //    vPos = (pFirst.vLastPos + pSecond.vLastPos) * 0.5f, //�߰� ����
        //    fValue = fRotateDelta, //��ġ ������ ��ȭ
        //    bOverUI = pFirst.bOverUI || pSecond.bOverUI
        //});
    }

    private void moved_touch(in Touch _tTouch, List<tTouchEvent> _listTouchEvent)
    {
        int iID = _tTouch.finger.index;
        if (m_listTouchOverUI[iID] == true)
            return;

        //���ſ� �ֵ��Դ� ��ġ�ð��� ��
        float fDeltaTime = (float)(_tTouch.time - _tTouch.startTime);
        Vector2 vDeltaDist = _tTouch.screenPosition - _tTouch.startScreenPosition;

        if (fDeltaTime >= m_fLongPressTime && vDeltaDist.magnitude < m_fDragStartDist)
        {
            add_touch_event(_listTouchEvent, "stay",
                Vector2.zero, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        else if (vDeltaDist.magnitude >= m_fDragStartDist) //�����Ÿ� �̻� �̵� �巡��
        {
            add_touch_event(_listTouchEvent, "drag",
                vDeltaDist, fDeltaTime, m_listTouchOverUI[iID], iID);
        }
    }

    private void ended_touch(in Touch _tTouch, List<tTouchEvent> _listTouchEvent)
    {
        int iID = _tTouch.finger.index;
        if (m_listTouchOverUI[iID] == true)
        {
            m_listTouchOverUI[iID] = false;
            return;
        }

        float fDeltaTime = (float)(_tTouch.time - _tTouch.startTime);
        Vector2 vDeltaDist = _tTouch.screenPosition - _tTouch.startScreenPosition;

        //ª�� �ð�, ���� �̵���
        if (fDeltaTime <= m_fTapMaxTime && vDeltaDist.magnitude < m_fTapMaxMove)                 
        {
            add_touch_event(_listTouchEvent, "tap",
                Vector2.zero, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        //ū �̵���
        else if (vDeltaDist.magnitude >= m_fSwipeMinDist) //�������� �ν�
        {
            add_touch_event(_listTouchEvent, "swipe", 
                vDeltaDist, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        m_listTouchOverUI[iID] = false; 
    }

    private void began_touch(in Touch _tTouch, UIRayCaster _pRayCaster)
    {
        m_listTouchOverUI[_tTouch.finger.index] = _pRayCaster.IsOverUI(_tTouch.screenPosition);
    }
}
