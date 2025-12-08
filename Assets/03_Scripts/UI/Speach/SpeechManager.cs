using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{
    [SerializeField] private SpeechTyper m_pSpeechTyper;

    static public SpeechManager m_Instance;

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
    }


    public void SettingSpeech(SOSpeech m_pTarget)
    {
        //m_pSpeechTyper.ShowText();
    }
}
