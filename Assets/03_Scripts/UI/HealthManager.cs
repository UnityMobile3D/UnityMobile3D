using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_pHealthText;
    [SerializeField] private Image m_pHealthImage;
    [SerializeField] private Health m_pPlayerHealth;

    public void PlayerTakeDamage()
    {
        float fHPRatio = 0.0f;
        if (m_pPlayerHealth.CurrentHP == 0)
            fHPRatio = 0.0f;

        fHPRatio = (float)m_pPlayerHealth.CurrentHP / m_pPlayerHealth.MaxHP;

        if(fHPRatio == 0.0f)
            m_pHealthImage.fillAmount = 0;
        else
            m_pHealthImage.fillAmount = fHPRatio;

        m_pHealthText.text = $"{(int)(fHPRatio * 100)} / {m_pPlayerHealth.CurrentHP}";
    }

    public void Awake()
    {
        PlayerTakeDamage();
    }
}
