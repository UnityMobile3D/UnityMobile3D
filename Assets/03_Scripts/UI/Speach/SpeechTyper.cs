using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechTyper : MonoBehaviour
{
    [SerializeField] private TMP_Text m_pText;         
    [SerializeField] private float charsPerSecond = 20f;

    private float m_fCurTime = 0.0f;

    private string m_strFullString = "";
    private float m_fInterval = 0.0f;
    private Coroutine m_pTypingCoroutine;
 
    public void ShowText(string _strMessage)
    {
        m_fInterval = 1.0f / charsPerSecond;

        m_strFullString = _strMessage;

        m_pText.text = m_strFullString;
        

        m_pText.ForceMeshUpdate();
        m_pText.maxVisibleCharacters = 0;

        // 이전 타이핑 중이면 정지
        if (m_pTypingCoroutine != null)
            StopCoroutine(m_pTypingCoroutine);

        // 새로 시작
        m_pTypingCoroutine = StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        int iTotalChar = m_pText.textInfo.characterCount;
        int iVisibleCount = 0;

        while (iVisibleCount < iTotalChar)
        {
            m_fCurTime += Time.deltaTime;
            while (m_fCurTime >= m_fInterval && iVisibleCount < iTotalChar)
            {
                m_fCurTime -= m_fInterval;
                iVisibleCount++;
                m_pText.maxVisibleCharacters = iVisibleCount;
            }
            yield return null;

        }

        m_pTypingCoroutine = null;
    }
}
