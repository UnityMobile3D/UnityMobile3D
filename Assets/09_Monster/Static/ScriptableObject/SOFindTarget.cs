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

            //거리 보다 멀다면 failed
            _pBB.DistanceToTarget = GlobalAction.GetDisttance(_pBB.Self.position, _pBB.Target.position);
            if(_pBB.DistanceToTarget > m_pFindTarget.m_fMaxDistance)
            {
                _pBB.Agent.isStopped = true;
                return STATE.FAILED;
            }

            // 지정된 각도보다 안된다면 failed
            float fAngle = GlobalAction.GetDirection(_pBB.Self.forward, _pBB.Self.position, _pBB.Target.position);
            if (fAngle > m_pFindTarget.m_fFOV * 0.5f)
            {
                _pBB.Agent.isStopped = true;
                return STATE.FAILED;
            }


            _pBB.Agent.isStopped = false;
            return STATE.SUCCESS;
        }


    }

}
