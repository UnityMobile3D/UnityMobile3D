using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

using STATE = INode.STATE;
public interface INode
{
    public enum STATE { RUN, SUCCESS, FAILED }
    public STATE Evaluate();
}

//각 Action에 필드를 중복 보관하지 않아도 되고, 다른 노드 간 정보 전달이 쉬워진다
//기본적으로 필요한 정보 보관
public class BlackNode
{
    public Transform Self;
    public Transform Target;
    public float DistanceToTarget;
    public float HpRatio;
    public Vector3 PrevSeenPosition;
}

//가장 하위 노드 (리프 노드)
public class ActionNode : INode
{
    public Func<Blackboard, STATE> m_pFunc;
    private readonly Blackboard m_pBlackBoard;
    public ActionNode(Blackboard _pBlackBoard, Func<Blackboard,STATE> _pFunc)
    {
        m_pBlackBoard = _pBlackBoard;
        m_pFunc = _pFunc;
    }

    public STATE Evaluate()
    {
        if(m_pFunc != null)
            return m_pFunc(m_pBlackBoard);

        return STATE.FAILED;
    }
}

public class SelectorNode
{
    private readonly List<INode> m_listNodes = new List<INode>();
    private int m_iCurrentNode = 0;
    public void AddNode(INode _pNode)
    {
        m_listNodes.Add(_pNode);
    }

    //이러면 RUN 중이던 자식이 있어도 매 프레임 앞선 노드에 가로막혀 깜빡임,
    //우왕좌왕이 발생 때문에 현제 노드 인덱스 캐싱
    public STATE Evaluate()
    {
        for (int i = 0; i < m_listNodes.Count; ++i)
        {
            STATE eResutState = m_listNodes[i].Evaluate();
            if (eResutState == STATE.SUCCESS)
            {
                m_iCurrentNode = i;
                return STATE.SUCCESS;
            }

            else if (eResutState == STATE.RUN)
            {
                m_iCurrentNode = i;
                return STATE.RUN;
            }
        }

        m_iCurrentNode = 0;
        return STATE.FAILED;
    }
}


public class SequenceNode : INode
{
    private readonly List<INode> m_listNodes = new List<INode>();
    private int m_iCurrentNode = 0;
    public void AddNode(INode _pNode)
    {
        m_listNodes.Add(_pNode);
    }
    //모든 노드가 성공해야지 성공, 하나라도 실패하면 실패, 중간에 RUN이 나오면 그 노드부터 다시 시작
    public STATE Evaluate()

    {
        for (int i = m_iCurrentNode; i < m_listNodes.Count; ++i)
        {
            STATE eResutState = m_listNodes[i].Evaluate();
            if (eResutState == STATE.FAILED)
            {
                m_iCurrentNode = 0;
                return STATE.FAILED;
            }

            else if (eResutState == STATE.RUN)
            {
                m_iCurrentNode = i;
                return STATE.RUN;
            }
        }

        m_iCurrentNode = 0;
        return STATE.SUCCESS;
    }
}
