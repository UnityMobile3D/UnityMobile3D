using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATE = INode.STATE;

[CreateAssetMenu(menuName = "SO/ActionNode/FindTarget")]
public class SOFindTarget : SONode
{
    public float m_fFOV = 90f;
    public float m_fMaxDistance = 10f;

    public override INode CreateRuntime()
    {
        return new FindTargetRuntime(this);
    }

    private class FindTargetRuntime : INode
    {
        private SOFindTarget m_pFindTarget = null;
        public FindTargetRuntime(SOFindTarget _pOwner)
        {
            m_pFindTarget = _pOwner;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            if (_pBB.Agent == null || _pBB.Target == null)
                return STATE.FAILED;

            //각도, 거리 체크
            Vector3 vDiff = _pBB.Target.position - _pBB.Self.position;
            if (vDiff.magnitude > m_pFindTarget.m_fMaxDistance)
                return STATE.FAILED;

            float fAngle = Vector3.Angle(_pBB.Self.forward, vDiff.normalized);
            if (fAngle > m_pFindTarget.m_fFOV * 0.5f)
                return STATE.FAILED;

            return STATE.SUCCESS;
        }
    }

}
