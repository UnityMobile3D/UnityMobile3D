using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;


[CreateAssetMenu(menuName = "SO/ActionNode/SelectAttack")]

public class SOSelectAttack : SONode
{
    public override INode CreateRuntime()
    {
        return new SelectAttackRunTime(this);
    }

    private class SelectAttackRunTime : INode
    {
        private SOSelectAttack m_pAttackTarget = null;
        private float m_fCurTime;
        public SelectAttackRunTime(SOSelectAttack _pOwner)
        {
            m_pAttackTarget = _pOwner;
            m_fCurTime = 0.0f;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            int iAttackIdx = _pBB.CooldownModule.AnyReady();
            if (iAttackIdx == -1)
                return STATE.FAILED;

            _pBB.CooldownModule.StartCooldown(iAttackIdx);
            _pBB.AnimBridge.SetAttack(iAttackIdx,true);

            return STATE.SUCCESS;
        }
    }

}
