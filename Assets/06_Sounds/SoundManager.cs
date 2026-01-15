using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager m_Instance = null;

    [Header("BGM")]
    [SerializeField] private AudioSource m_pBgmSource;

    [Header("SFX Pool")]
    [SerializeField] private int m_iSfxPoolCount = 10;
    [SerializeField] private AudioSource m_pSfxPrefab;

    private List<AudioSource> m_listSfxSource = new List<AudioSource>();
    private int m_iSfxIndex = 0;

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;

        BuildSfxPool();
    }

    private void BuildSfxPool()
    {
        m_listSfxSource.Clear();

        for (int i = 0; i < m_iSfxPoolCount; ++i)
        {
            AudioSource pSrc;
            if (m_pSfxPrefab != null)
            {
                pSrc = Instantiate(m_pSfxPrefab, transform);
            }
            else
            {
                GameObject go = new GameObject("SFX_" + i);
                go.transform.SetParent(transform);
                pSrc = go.AddComponent<AudioSource>();
            }

            pSrc.playOnAwake = false;
            pSrc.loop = false;
            m_listSfxSource.Add(pSrc);
        }
    }


    public void PlayBgm(SOAudio _pAudio)
    {
        if (_pAudio == null || _pAudio.Clips.Count == 0)
            return;

        AudioClip pClip = _pAudio.Clips[0];
        m_pBgmSource.outputAudioMixerGroup = _pAudio.OutputGroup;
        m_pBgmSource.clip = pClip;
        m_pBgmSource.loop = true;
        m_pBgmSource.volume = _pAudio.Volume;
        m_pBgmSource.pitch = 1.0f;
        m_pBgmSource.spatialBlend = 0.0f;
        m_pBgmSource.Play();
    }

    public void StopBgm()
    {
        m_pBgmSource.Stop();
        m_pBgmSource.clip = null;
    }

    public int PlaySfx(SOAudio _pAudio, Transform _pTransform)
    {
        if (_pAudio == null || _pAudio.Clips.Count == 0)
            return -1;

        int iSrcIdx = GetNextSfxSourceIdx();

        AudioSource pSrc = m_listSfxSource[iSrcIdx];

        int iClipIdx = Random.Range(0, _pAudio.Clips.Count);
        pSrc.clip = _pAudio.Clips[iClipIdx];
        pSrc.outputAudioMixerGroup = _pAudio.OutputGroup;

        pSrc.volume = _pAudio.Volume;
        pSrc.pitch = Random.Range(_pAudio.PitchMin, _pAudio.PitchMax);

        if (_pAudio.spatial3D && _pTransform != null)
        {
            pSrc.transform.position = _pTransform.position;
            pSrc.spatialBlend = _pAudio.SpatialBlend;
        }
        else
            pSrc.spatialBlend = 0.0f;

        pSrc.Play();

        return iSrcIdx;
    }
    public void StopSfx(int _iSrcIdx)
    {
        if (_iSrcIdx >= m_iSfxPoolCount || _iSrcIdx < 0)
            return;

        m_listSfxSource[_iSrcIdx].Stop();
    }

    private int GetNextSfxSourceIdx()
    {
        if (m_listSfxSource.Count == 0)
            return -1;

        int iClipIdx = m_iSfxIndex;
        ++m_iSfxIndex;
        if (m_iSfxIndex >= m_listSfxSource.Count)
            m_iSfxIndex = 0;

        return iClipIdx;
    }
}
