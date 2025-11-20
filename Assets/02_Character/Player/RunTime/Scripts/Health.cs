using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField] private int m_iHP = 100;
    [SerializeField] private int m_iMaxHP = 100;
    [SerializeField] private int m_iDefense = 1;
    [SerializeField] private int m_iSpeed = 1;
    [SerializeField] private int m_iAttackPower = 10;
    [SerializeField] private int m_iAttackSpeed = 1;

    public int CurrentHP { get { return m_iHP; } }
    public int MaxHP { get { return m_iMaxHP; } }

    public PED TakeDamageEvent = null;
    public PED DeadEvent = null;

    public void TakeDamage(int _iDamage)
    {
        m_iHP -= (_iDamage - m_iDefense);
        if (m_iHP <= 0)
            m_iHP = 0;

        TakeDamageEvent?.Invoke();

        if (m_iHP <= 0)
            DeadEvent?.Invoke();
    }
    public void Heal(int _iAmount)
    {
        m_iHP += _iAmount;
        if (m_iHP > m_iMaxHP)
            m_iHP = m_iMaxHP;
    }
    public bool IsDead()
    {
        return m_iHP <= 0;
    }

    public void AddAttackPower(int _iAttackIncrease)
    {
        m_iAttackPower += _iAttackIncrease;
    }
}


public interface IHealth
{
    int CurrentHP { get; }
    int MaxHP { get; }

    void TakeDamage(int _iDamage);
    void Heal(int _iAmount);
    bool IsDead();
}



