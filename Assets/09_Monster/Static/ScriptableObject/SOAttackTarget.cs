using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATE = INode.STATE;


[CreateAssetMenu(menuName = "SO/ActionNode/AttackTarget")]
public class SOAttackTarget : SONode
{
    public float m_fAttackRange = 2.0f;

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
            if (_pBB.Target == null)
                return STATE.FAILED;

            //만약 공격이 끝났는데도 계속 있다면 run, 공격범위를 벗어나면 failed

            return STATE.RUN;
        }
    }
}