using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUI : MonoBehaviour
{

    [SerializeField] private ButtonUI m_pSkillButton = null;
    [SerializeField] private ButtonUI m_pInfoButton = null;
    [SerializeField] private ButtonUI m_pShopButton = null;

    [SerializeField] private ButtonUI m_pOptionButton = null;
    [SerializeField] private GameObject m_pContent = null;
    private void Awake()
    {
        m_pSkillButton.OnClickEvt += visible_skill;
        m_pInfoButton.OnClickEvt += visible_infop;
        m_pShopButton.OnClickEvt += visible_infop;

        m_pOptionButton.OnClickEvt += () =>
        {
            m_pContent.SetActive(!m_pContent.activeSelf);
        };
    }

    
    private void visible_skill()
    {
        DataService.m_Instance.SetVisibleContainer(eContainerType.SkillTree, true);
    }
    private void visible_infop()
    {
        DataService.m_Instance.SetVisiblePlayerInfo(true);
    }

    private void visivle_shop()
    {
        DataService.m_Instance.SetVisibleContainer(eContainerType.Store, true);
    }
}
