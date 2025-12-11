using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IRaycastEvent
{
    public void Execute()
    {
        //테두리
    }
}


public class NPC : MonoBehaviour
{
    [SerializeField] private SOSpeech m_pSpeech;
    //SpeechManager두기

    private Animator m_pAnimator;
    [SerializeField] private LayerMask m_tPlayerMask;

    private void Awake()
    {
        m_pAnimator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((m_tPlayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            Vector3 vQuestPos = transform.position + transform.up * 2.0f;
            SpeechManager.m_Instance.ShowWSpeechUI(vQuestPos, m_pSpeech);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((m_tPlayerMask.value & (1 << collision.gameObject.layer)) != 0)
        {
            SpeechManager.m_Instance.CloseSpeechdUI();
        }
    }


}
