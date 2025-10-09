using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//Ondestory는 에디터에서 안됨 ExecuteAlways를 붙여야함

//[ExecuteAlways]
public class CoolDownView : MonoBehaviour
{

    /*
[   SerializeField] private GameObject m_pOverlayObject;
    하지만 new GameObject()로 런타임에 만든 건 저장이 안 되니 결국 null 됨
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

    //컴포넌트 부착시 실행
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
            //RectTr, Image 가진 오버레이 이미지 생성후 자식으로 
            m_pOverlayObject = new GameObject(m_sOverlayName, typeof(RectTransform), typeof(Image));
            m_pOverlayObject.transform.SetParent(transform);
            m_pOverlayImage = m_pOverlayObject.GetComponent<Image>();

            //이미지 속성 
            m_pOverlayImage.sprite = GetComponent<Image>().sprite;
            m_pOverlayImage.type = Image.Type.Filled;
            m_pOverlayImage.fillMethod = Image.FillMethod.Vertical;
            m_pOverlayImage.fillOrigin = (int)Image.OriginVertical.Bottom; //아래서 위로 채워지게
            m_pOverlayImage.fillAmount = 0f;
            m_pOverlayImage.color = new Vector4(0.5f, 0.5f, 0.5f, 0.5f); //회색
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