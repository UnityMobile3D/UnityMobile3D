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

    private const float m_fTapMaxTime = 0.2f;  // 탭 최대 지속시간
    private const float m_fTapMaxMove = 3.0f;   // 탭 허용 이동량
    private const float m_fLongPressTime = 0.45f; // 롱프레스 최소 시간
    private const float m_fSwipeMinDist = 5.0f;   // 스와이프 최소 거리
    private const float m_fDragStartDist = 1.0f;   // 드래그 시작 거리


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

        //페어가 바뀌지 않게 가장 처음 2개를 우선으로 정렬해서
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

        Debug.Log($"ID {_iID} 타입 {_strStateType} 값 : {_fValue} " +
            $"이동량 : {_vDelta} UI {_bOverUI}" );
    }

    private void track_multi_touch(List<tTouchEvent> _listTouchEvent, in Touch _tFirstTouch, in Touch _tSecondTouch)
    {
        int iFirstID = _tFirstTouch.finger.index;
        int iSecondID = _tSecondTouch.finger.index;

        //여기 바꾸기 _tTouch로만으로도 충분 Tack을 안쓰고 List<bool> 값으로 overUi만 검사
        float fCurDist = (_tFirstTouch.screenPosition - _tSecondTouch.screenPosition).magnitude;                //현재 두 손가락 거리
        float fStartDist = (_tFirstTouch.startScreenPosition - _tSecondTouch.startScreenPosition).magnitude;    //시작 두 손가락 거리
        float fScale = fCurDist / fStartDist;                                                       //핀치 스케일 변화

        add_touch_event(_listTouchEvent, "pinch", 
            /*(pFirst.vLastPos + pSecond.vLastPos) * 0.5f,*/ Vector2.zero, fScale,
            m_listTouchOverUI[iFirstID] && m_listTouchOverUI[iSecondID], iFirstID | iSecondID);

        //Vector2 vStartVector = pSecond.vStartPos - pFirst.vStartPos;        //시작 시 두 손가락 터치한 지점 벡터
        //Vector2 vCurVector = pSecond.vLastPos - pFirst.vLastPos;            //현재 두 손가락 터치한 지점 벡터
        //
        //
        //float fStartAngle = Mathf.Atan2(vStartVector.y, vStartVector.x) * Mathf.Rad2Deg; //시작 각도 라디안에서 디그리로
        //float fCurAngle = Mathf.Atan2(vCurVector.y, vCurVector.x) * Mathf.Rad2Deg; //현재 각도 라디안에서 디그리로
        //float fRotateDelta = Mathf.DeltaAngle(fStartAngle, fCurAngle); //회전 변화량(도 단위)

        //_listTouchEvent.Add(new tTouchEvent
        //{
        //    strType = "rotate",
        //    iPointerIDA = pFirst.iID,
        //    iPointerIDB = pSecond.iID,
        //    vPos = (pFirst.vLastPos + pSecond.vLastPos) * 0.5f, //중간 지점
        //    fValue = fRotateDelta, //핀치 스케일 변화
        //    bOverUI = pFirst.bOverUI || pSecond.bOverUI
        //});
    }

    private void moved_touch(in Touch _tTouch, List<tTouchEvent> _listTouchEvent)
    {
        int iID = _tTouch.finger.index;
        if (m_listTouchOverUI[iID] == true)
            return;

        //과거에 있들어왔던 터치시간과 비교
        float fDeltaTime = (float)(_tTouch.time - _tTouch.startTime);
        Vector2 vDeltaDist = _tTouch.screenPosition - _tTouch.startScreenPosition;

        if (fDeltaTime >= m_fLongPressTime && vDeltaDist.magnitude < m_fDragStartDist)
        {
            add_touch_event(_listTouchEvent, "stay",
                Vector2.zero, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        else if (vDeltaDist.magnitude >= m_fDragStartDist) //일정거리 이상 이동 드래그
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

        //짧은 시간, 적은 이동량
        if (fDeltaTime <= m_fTapMaxTime && vDeltaDist.magnitude < m_fTapMaxMove)                 
        {
            add_touch_event(_listTouchEvent, "tap",
                Vector2.zero, fDeltaTime, m_listTouchOverUI[iID], iID);
        }

        //큰 이동량
        else if (vDeltaDist.magnitude >= m_fSwipeMinDist) //스와이프 인식
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
