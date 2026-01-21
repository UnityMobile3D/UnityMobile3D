using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_listOption = new List<GameObject>();

    public void ClickOption(GameObject _pGameObject)
    {
        for(int i = 0; i<m_listOption.Count; ++i)
        {
            if (_pGameObject == m_listOption[i])
                m_listOption[i].SetActive(true);
            else
                m_listOption[i].SetActive(false);
        }
    }
}
