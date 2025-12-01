using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBridge : MonoBehaviour
{
    public Animator m_pAnimator = null;

    [SerializeField] private List<string> ListAttack = new List<string>() { "Attack" };
    [SerializeField] private string Run = "Run";
    [SerializeField] private string Hit = "Hit";
    [SerializeField] private string Dead = "Dead";

    private int m_iSpeedHash;
    private List<int> m_listAttack = new List<int>();
    private int m_iMoveHash;
    private int m_iHitHash;
    private int m_iDeadHash;

    Dictionary<string, int> m_hashNameToId = new Dictionary<string, int>();
    public void Init(Animator _pAnim)
    {
        if(m_pAnimator == null)
            m_pAnimator = _pAnim;


        for(int i = 0; i<ListAttack.Count; ++i)
        {
             int iHashID = Animator.StringToHash(ListAttack[i]);
            m_hashNameToId[ListAttack[i]] = iHashID;
            m_listAttack.Add(iHashID);
        }
      
        m_iMoveHash = Animator.StringToHash(Run);
        m_hashNameToId[Run] = m_iMoveHash;

        m_iHitHash = Animator.StringToHash(Hit);
        m_hashNameToId[Hit] = m_iHitHash;

        m_iDeadHash = Animator.StringToHash(Dead);
        m_hashNameToId[Dead] = m_iDeadHash;
    }


    public void SetAttack(int _iIdx, bool _bOn )
    {
        m_pAnimator.SetBool(m_listAttack[_iIdx], _bOn);
    }
    public void SetAttack(in string _strName, bool _bOn)
    {
        m_pAnimator.SetBool(_strName, _bOn);
    }
    public void SetRun()
    {
        m_pAnimator.SetTrigger(Run);
    }
    public void SetRun(bool _bOn)
    {
        m_pAnimator.SetBool(Run, _bOn);
    }

    public void SetHit()
    {
        m_pAnimator.SetTrigger(Hit);
    }
    public void SetDead()
    {
        m_pAnimator.SetTrigger(Dead);
    }

    public bool CurrentClipPlayedOnce(in string _strName , int _iLayer = 0)
    {
        //전이상태라면 false
        if (m_pAnimator.IsInTransition(_iLayer)) 
            return false;

        int iHashId = -1;
        if (m_hashNameToId.TryGetValue(_strName, out iHashId) == false)
            return false;

        var tInfo = m_pAnimator.GetCurrentAnimatorStateInfo(_iLayer);
        if (tInfo.shortNameHash == iHashId &&
            tInfo.normalizedTime >= 1.0)
            return true;
        
        return false;
    }

    public bool CurrentClipPlayedAttackOnce(int _iIdx, int _iLayer = 0)
    {
        //전이상태라면 false
        if (m_pAnimator.IsInTransition(_iLayer))
            return false;

        int iHashId = -1;
        if (m_hashNameToId.TryGetValue(ListAttack[_iIdx], out iHashId) == false)
            return false;

        var tInfo = m_pAnimator.GetCurrentAnimatorStateInfo(_iLayer);
        if (tInfo.shortNameHash == iHashId &&
            tInfo.normalizedTime >= 1.0)
            return true;

        return false;
    }

}
