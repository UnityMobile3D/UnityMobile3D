using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHelper : MonoBehaviour
{
    [SerializeField] private SOPortal m_pStartScene = null;
    
    public async void Enter()
    {
        if (m_pStartScene == null)
            return;

        await GameSceneManager.m_Instance.StartScene(m_pStartScene);
    }
}
