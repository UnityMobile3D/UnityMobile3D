using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWhileCasting : MonoBehaviour
{
    [SerializeField] public float m_fMaxTime = 1.0f;
    private float m_fCurTime = 0.0f;

    [SerializeField] private Vector3 m_vGoalScale = Vector3.one;
    [SerializeField] private Vector3 m_vStartScale = Vector3.one;

    [SerializeField] private PED m_pCompleteEvent;

    private void OnEnable()
    {
        m_fCurTime = 0.0f;
        transform.localScale= m_vStartScale;
    }
 

    private void Update()
    {
        if (m_fCurTime >= m_fMaxTime)
        {
            m_pCompleteEvent?.Invoke();
            return;
        }

        m_fCurTime += Time.deltaTime;
        float fRatio = m_fCurTime / m_fMaxTime;

        transform.localScale = Vector3.Lerp(m_vStartScale, m_vGoalScale, fRatio);
    }
}
