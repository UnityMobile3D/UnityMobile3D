using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using STATE = INode.STATE;


//셀렉트 노드에서 가장 처음으로 시작할 노드
[CreateAssetMenu(menuName = "SO/ActionNode/Idle")]

public class SOIdleAction : SONode
{
    public float m_fFindRange = 10.0f;
    public override INode CreateRuntime()
    {
        return new IdleActionRuntime(this);
    }

    private class IdleActionRuntime : INode
    {
        private SOIdleAction pIdleAction = null;
       
        public IdleActionRuntime(SOIdleAction _pOwner)
        {
            pIdleAction = _pOwner;
        }

        //거리만 확인
        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            if (_pBB.Target == null)
                return STATE.FAILED;

            float fDist = Vector3.Distance(_pBB.Self.position, _pBB.Target.position);
            _pBB.DistanceToTarget = fDist;

            if(fDist <=  pIdleAction.m_fFindRange)
            {
                _pBB.AnimBridge.SetMove(true);
                return STATE.FAILED;
            }

            return STATE.SUCCESS;
        }
    }
}