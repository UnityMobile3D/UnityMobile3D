using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //시네머신 기준
    [SerializeField] List<CinemachineVirtualCamera> m_listCamera = new();
    [SerializeField] float m_fMoveSpeed = 10.0f;
    [SerializeField] float m_fZoomSpeed = 1.0f;
    private CinemachineVirtualCamera m_pMainCamera = null;
    private bool m_bFreeView = false;

    private Vector3 m_vBaseFollowOffset;
 
    private CinemachineTransposer m_pMainTransposer;
    public void Awake()
    {
        m_pMainTransposer = Camera.main.GetComponentInChildren<CinemachineTransposer>();
    }

    public void MoveCameraPosition()
    {
        if (m_bFreeView == false)
            return;

        Vector2 vDelta = InputManager.m_Instance.PointerState.vDelta;
        if (vDelta == Vector2.zero)
            return;

        Vector3 vOffset = m_pMainTransposer.m_FollowOffset;
        vOffset.x -= vDelta.x * m_fMoveSpeed * Time.deltaTime;
        vOffset.z -= vDelta.y * m_fMoveSpeed * Time.deltaTime;

        m_pMainTransposer.m_FollowOffset = vOffset;

    }

    public void ZoomCamera()
    {
        if (m_bFreeView == false)
            return;
        //핀치면
        float fZoom = InputManager.m_Instance.PointerState.fZoomDelta;
        
        if(fZoom > 0.0f)
        {
            Vector3 vOffset = m_pMainTransposer.m_FollowOffset;

            // 현재 거리
            float fDist = vOffset.magnitude;

            // 거리 증가/감소
            fDist -= fZoom * m_fZoomSpeed * Time.deltaTime;

            // 최소 / 최대 거리 제한
            fDist = Mathf.Clamp(fDist, 4f, 30f);
         
            // 방향을 유지한 상태에서 거리 재적용
            vOffset = vOffset.normalized * fDist;

            m_pMainTransposer.m_FollowOffset = vOffset;
        }
    }
    public void FreeViewButton()
    {
        if(m_bFreeView == false)
        {
            m_vBaseFollowOffset = m_pMainTransposer.m_FollowOffset;

            m_bFreeView = true;
        }
        else
        {
            m_bFreeView = false;
            m_pMainTransposer.m_FollowOffset = m_vBaseFollowOffset;
        }
    }
   
}
