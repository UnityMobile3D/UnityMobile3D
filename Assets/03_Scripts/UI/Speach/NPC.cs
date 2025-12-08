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
    [SerializeField] private List<SOSpeech> m_listSpeech;
    //SpeechManager두기

    private Animator m_pAnimator;
    private LayerMask m_tPlayerMask;

    private void Awake()
    {
        m_pAnimator = GetComponent<Animator>();
        m_tPlayerMask |= 1 << LayerMask.GetMask("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((m_tPlayerMask & (1 << collision.gameObject.layer)) != 0)
        {

        }
    }
 

}
