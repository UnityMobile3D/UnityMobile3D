using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATE = INode.STATE;


[CreateAssetMenu(menuName = "SO/ActionNode/AttackTarget")]
public class SOAttackTarget : SONode
{
    public override INode CreateRuntime()
    {
        return new AttackTargetRuntime(this);
    }

    private class AttackTargetRuntime : INode
    {
        private SOAttackTarget m_pAttackTarget = null;
        private float m_fCurTime;
        public AttackTargetRuntime(SOAttackTarget _pOwner)
        {
            m_pAttackTarget = _pOwner;
            m_fCurTime = 0.0f;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            //현재 공격 모션이 끝났다면
            if (_pBB.Target == null || _pBB.AnimBridge.CurrentClipPlayedAttackOnce(_pBB.CooldownModule.TargetIdx))
            {
                _pBB.Agent.isStopped = false;
                _pBB.AnimBridge.SetAttack(_pBB.CooldownModule.TargetIdx, false);

                return STATE.FAILED;
            }

            else
                return STATE.RUN;
        }
    }
}