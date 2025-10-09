using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//Ondestory�� �����Ϳ��� �ȵ� ExecuteAlways�� �ٿ�����

//[ExecuteAlways]
public class CoolDownView : MonoBehaviour
{

    /*
[   SerializeField] private GameObject m_pOverlayObject;
    ������ new GameObject()�� ��Ÿ�ӿ� ���� �� ������ �� �Ǵ� �ᱹ null ��
     */

    [SerializeField] private Slot m_pOwner; 
    private Image m_pOverlayImage;
    private GameObject m_pOverlayObject;
    [SerializeField] string m_sOverlayName = "CooldownOverlay";

    private float m_fMaxCoolTime = 0f;
    private float m_fCurCoolTime = 0f;

    private void Awake()
    {
        m_pOwner = GetComponent<Slot>();
        if (m_pOwner == null)
            Destroy(this);
        else
        {
            m_pOwner.SetCoolDownView(this);
            create_overlay();
        }
    }

    //������Ʈ ������ ����
    private void Reset()
    {
        if (m_pOwner == null)
        {
            Debug.Assert(false, "CooldownView must be have Slot");
            Destroy(this);

            return;
        }
    }

    private void OnValidate()
    {
        if(!Application.isPlaying)
        {
            create_overlay();
        }
    }


    private void Update()
    {
        if(m_pOwner.IsCanUse == false)
        {
            m_fCurCoolTime += Time.deltaTime;
            float m_fRatio = m_fCurCoolTime / m_fMaxCoolTime;
            UpdateCoolTime(m_fRatio);
        }
    }

    private void create_overlay()
    {
        Transform pOverlay = transform.Find(m_sOverlayName);
        if (pOverlay != null)
        {
            m_pOverlayObject = pOverlay.gameObject;
            m_pOverlayImage = pOverlay.GetComponent<Image>();
        }
        else
        {
            //RectTr, Image ���� �������� �̹��� ������ �ڽ����� 
            m_pOverlayObject = new GameObject(m_sOverlayName, typeof(RectTransform), typeof(Image));
            m_pOverlayObject.transform.SetParent(transform);
            m_pOverlayImage = m_pOverlayObject.GetComponent<Image>();

            //�̹��� �Ӽ� 
            m_pOverlayImage.sprite = GetComponent<Image>().sprite;
            m_pOverlayImage.type = Image.Type.Filled;
            m_pOverlayImage.fillMethod = Image.FillMethod.Vertical;
            m_pOverlayImage.fillOrigin = (int)Image.OriginVertical.Bottom; //�Ʒ��� ���� ä������
            m_pOverlayImage.fillAmount = 0f;
            m_pOverlayImage.color = new Vector4(0.5f, 0.5f, 0.5f, 0.5f); //ȸ��
            m_pOverlayImage.raycastTarget = false;

            RectTransform pRect = m_pOverlayObject.GetComponent<RectTransform>();
            RectTransform pOwnerRect = GetComponent<RectTransform>();

            pRect.sizeDelta = pOwnerRect.sizeDelta;
            pRect.localScale = Vector3.one;
            pRect.anchoredPosition = Vector2.zero;
        }
    }

    public void UpdateCoolTime(float _fCoolTimeRatio)
    {
        m_pOverlayImage.fillAmount = 1.0f - _fCoolTimeRatio;
        if (m_pOverlayImage.fillAmount <= 0.0f)
        {
            m_fCurCoolTime = 0.0f;
            m_pOwner.SetUse(true);
        }
    }
    public void SetCoolTime(float _fTime)
    {
        m_fCurCoolTime = 0.0f;
        m_fMaxCoolTime = _fTime;
        m_pOverlayImage.fillAmount = 0.0f;
    }

}