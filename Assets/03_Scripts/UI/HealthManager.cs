using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_pHealthText;
    [SerializeField] private Image m_pHealthImage;
    [SerializeField] private IHealth m_IPlayerStatus;

    public static HealthManager m_Instance = null;
    public void Awake()
    {
        if (m_Instance != null)
            Destroy(m_Instance);

        m_Instance = this;
    }
    public void Start()
    {
        m_IPlayerStatus = GameManager.m_Instance.Player.GetComponent<IHealth>();
        PlayerTakeDamage();
    }

    public void PlayerTakeDamage()
    {
        float fHPRatio = 0.0f;
        if (m_IPlayerStatus.CurrentHP == 0)
            fHPRatio = 0.0f;

        fHPRatio = (float)m_IPlayerStatus.CurrentHP / m_IPlayerStatus.MaxHP;

        if(fHPRatio == 0.0f)
            m_pHealthImage.fillAmount = 0;
        else
            m_pHealthImage.fillAmount = fHPRatio;

        m_pHealthText.text = $"{(int)(fHPRatio * 100)}% / {m_IPlayerStatus.MaxHP}";
    }
}
