using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BehaviorTree : MonoBehaviour
{
    [SerializeField] private SONode m_pRoot;
    private INode m_pRuntimeRoot = null;

    private Blackboard m_pBlackboard = null;

    public void Init(Blackboard _pBB)
    {
        m_pBlackboard = _pBB;

        m_pBlackboard.Self = transform;
        m_pBlackboard.Agent = GetComponent<NavMeshAgent>();
        m_pRuntimeRoot = m_pRoot.CreateRuntime();

        m_pBlackboard.AnimBridge.Init(GetComponent<Animator>());
    }
    
    public void Evaluate()
    {
        if (m_pRuntimeRoot == null)
            return;

        m_pRuntimeRoot.Evaluate(m_pBlackboard, Time.deltaTime);
    }
  
}
