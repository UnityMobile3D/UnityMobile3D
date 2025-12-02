using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_pHealthText;
    [SerializeField] private TextMeshProUGUI m_pMPText;
    [SerializeField] private Image m_pHealthImage;
    [SerializeField] private Image m_pMPImage;
    [SerializeField] private ObjectInfo m_IPlayerStatus;

    public static HealthManager m_Instance = null;
    public void Awake()
    {
        if (m_Instance != null)
            Destroy(m_Instance);

        m_Instance = this;
    }
    public void Start()
    {
        if(m_IPlayerStatus ==null)
            m_IPlayerStatus = GameManager.m_Instance.Player.GetComponent<ObjectInfo>();

        PlayerTakeDamage();
        PlayerConsumeMP();
    }

    public void PlayerTakeDamage()
    {
        float fHPRatio = 0.0f;
        if (m_IPlayerStatus.HP == 0)
            fHPRatio = 0.0f;
        else
            fHPRatio = (float)m_IPlayerStatus.HP / m_IPlayerStatus.MaxHp;

        if(fHPRatio == 0.0f)
            m_pHealthImage.fillAmount = 0;
        else
            m_pHealthImage.fillAmount = fHPRatio;

        m_pHealthText.text = $"{(int)(fHPRatio * 100)}% / {m_IPlayerStatus.MaxHp}";
    }

    public void PlayerConsumeMP()
    {
        float fMPRatio = 0.0f;
        if (m_IPlayerStatus.MP == 0)
            fMPRatio = 0.0f;
        else
            fMPRatio = (float)m_IPlayerStatus.MP / m_IPlayerStatus.MaxMp;

        if (fMPRatio == 0.0f)
            m_pHealthImage.fillAmount = 0;
        else
            m_pHealthImage.fillAmount = fMPRatio;

        m_pHealthText.text = $"{(int)(fMPRatio * 100)}% / {m_IPlayerStatus.MaxMp}";
    }
}
