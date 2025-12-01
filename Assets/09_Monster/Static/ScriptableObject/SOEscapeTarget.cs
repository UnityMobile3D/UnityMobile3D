using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using STATE = INode.STATE;

[CreateAssetMenu(menuName = "SO/ActionNode/EscapeTarget")]

public class SOEscapeTarget : SONode
{
    public float m_fEscapeDistance = 6.0f;
    public float m_fEscapeTime = 1.5f;
    public override INode CreateRuntime()
    {
        return new EscapeTargetRunTime(this);
    }

    private class EscapeTargetRunTime : INode
    {
        private SOEscapeTarget m_pEscapeTarget;
        private float m_fCurTime = 0.0f;
        public EscapeTargetRunTime(SOEscapeTarget _pEscapeTarget)
        {
            m_pEscapeTarget = _pEscapeTarget;
            m_fCurTime = _pEscapeTarget.m_fEscapeTime;
        }


        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            Vector2 vTargetPos = new Vector2(_pBB.Target.position.x, _pBB.Target.position.z);
            Vector2 vSelfPos = new Vector2(_pBB.Self.position.x, _pBB.Self.position.z);

            float fDistance = Vector2.Distance(vSelfPos, vTargetPos);

            m_fCurTime += _fDT;
            if (m_fCurTime >= m_pEscapeTarget.m_fEscapeTime)
            {
                _pBB.AnimBridge.SetRun(true);
                _pBB.Agent.isStopped = false;

                m_fCurTime = m_pEscapeTarget.m_fEscapeTime;
                Vector3 vDir = _pBB.Target.position - _pBB.Self.position;

                Vector3 vEscapePostition = _pBB.Self.position + vDir * m_pEscapeTarget.m_fEscapeDistance;
                if(NavMesh.SamplePosition(vEscapePostition, out NavMeshHit tHit,
                    0.1f, _pBB.Agent.areaMask) == true)
                    _pBB.Agent.SetDestination(tHit.position);

                return STATE.SUCCESS;
            }

            //안정거리까지 이동 확인
            if (fDistance >= m_pEscapeTarget.m_fEscapeDistance)
                return STATE.FAILED;
            else
                return STATE.RUN;
        }


    }
}