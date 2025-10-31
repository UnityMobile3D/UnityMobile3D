using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
public class Monster : MonoBehaviour
{
    [SerializeField] private Blackboard m_pBlackbard = new Blackboard();
    private BehaviorTree m_pBHTree = null;


    private void Awake()
    {
        m_pBlackbard.Self = transform;
        m_pBlackbard.Agent = GetComponent<NavMeshAgent>();
        m_pBlackbard.AnimBridge = GetComponent<AnimationBridge>();
        m_pBHTree= GetComponent<BehaviorTree>();

        m_pBHTree.Init(m_pBlackbard);

        
    }


    private void Update()
    {
        m_pBHTree.Evaluate();
    }

}
