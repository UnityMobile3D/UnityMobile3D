using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    private Animator m_pAnimator = null;

    [SerializeField] private string Speed = "Speed";
    [SerializeField] private string Attack = "Attack";
    [SerializeField] private string Move = "Move";
    [SerializeField] private string Hit = "Hit";
    [SerializeField] private string Dead = "Dead";
    
    public void Init(Animator _pAnim)
    {
        if(m_pAnimator == null)
            m_pAnimator = _pAnim;
    }

    public void SetSpeed(float fSpeed)
    {
        m_pAnimator.SetFloat(Speed, fSpeed);
    }
    public void SetAttack()
    {
        m_pAnimator.SetTrigger(Attack);
    }
    public void SetAttack(bool _bOn)
    {
        m_pAnimator.SetBool(Attack, _bOn);
    }
    public void SetMove()
    {
        m_pAnimator.SetTrigger(Move);
    }
    public void SetMove(bool _bOn)
    {
        m_pAnimator.SetBool(Move, _bOn);
    }

    public void SetHit()
    {
        m_pAnimator.SetTrigger(Hit);
    }
    public void SetDead()
    {
        m_pAnimator.SetTrigger(Dead);
    }

   

}
