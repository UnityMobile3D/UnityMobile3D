using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{

    [SerializeField] private SpeechTyper m_pSpeechTyper = null;
    private SOSpeech m_pSpeech = null;
    [SerializeField] private GameObject m_pWorldUI = null;
    [SerializeField] private Canvas m_pSpeechCanvas = null;

    static public SpeechManager m_Instance;

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        m_pSpeechTyper.Init();
    }

    public void ShowText()
    {   
        m_pSpeechCanvas.gameObject.SetActive(true);
        m_pSpeechTyper.StartSpeech(m_pSpeech);
    }
    public void ShowWSpeechUI(in Vector3 _vTargetPos , SOSpeech _pSpeech)
    {
        m_pWorldUI.SetActive(true);
        m_pWorldUI.transform.position = _vTargetPos;

        m_pSpeech = _pSpeech;
    }

    public void CloseSpeechdUI()
    {
        CloseWorldUI();
        m_pSpeechCanvas.gameObject.SetActive(false);
       
        m_pSpeech = null;
    }

    public void CloseWorldUI()
    {
        m_pWorldUI.SetActive(false);
    }
}
