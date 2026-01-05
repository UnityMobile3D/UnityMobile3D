using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player m_pPlayer;
    public Player Player { get { return m_pPlayer; } }

    public static GameManager m_Instance = null; 

  
    private void Awake()
    {
        if(m_Instance!=null)
            Destroy(m_Instance);

        m_Instance = this;

    }
}
