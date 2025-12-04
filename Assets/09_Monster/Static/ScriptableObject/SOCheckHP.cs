using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using STATE = INode.STATE;

[CreateAssetMenu(menuName = "SO/ActionNode/CheckHP")]

public class SOCheckHP : SONode
{
    public float m_fCheckHPRatio = 0.3f;
    public override INode CreateRuntime()
    {
        return new CheckHPRunTime(this);
    }

    private class CheckHPRunTime : INode
    {
        private SOCheckHP m_pOwner = null;
        public CheckHPRunTime(SOCheckHP _pOwner)
        {
            m_pOwner = _pOwner;
        }
        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            //내가 지정한 피 양보다 작으면 
            if(m_pOwner.m_fCheckHPRatio > _pBB.HpRatio)
            {
                return STATE.SUCCESS;
            }

            return STATE.FAILED;
        }


    }
}
