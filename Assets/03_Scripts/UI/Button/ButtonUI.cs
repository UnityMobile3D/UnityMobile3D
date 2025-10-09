using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Inspector에서 PointerEventData를 넘길 수 있게 하는 UnityEvent
[Serializable] public class PED : UnityEvent { }

public class ButtonUI : BaseUI,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerClickHandler
{
    //코드 바인딩용 델리게이트(원하면 사용) 
    public event Action OnEnterEvt;
    public event Action OnExitEvt;
    public event Action OnDownEvt;
    public event Action OnUpEvt;
    public event Action OnBeginDragEvt;
    public event Action OnDragEvt;
    public event Action OnEndDragEvt;
    public event Action OnClickEvt;

    // 인스펙터 바인딩용 
    [SerializeField] private PED onEnter;
    [SerializeField] private PED onExit;
    [SerializeField] private PED onDown;
    [SerializeField] private PED onUp;
    [SerializeField] private PED onBeginDrag;
    [SerializeField] private PED onDrag;
    [SerializeField] private PED onEndDrag;
    [SerializeField] private PED onClick;

    virtual public void OnPointerEnter(PointerEventData e)
    {
        onEnter?.Invoke();
        OnEnterEvt?.Invoke();
    }

    virtual public void OnPointerExit(PointerEventData e)
    {
        onExit?.Invoke();
        OnExitEvt?.Invoke();
    }

    virtual public void OnPointerDown(PointerEventData e)
    {
        onDown?.Invoke();
        OnDownEvt?.Invoke();
    }

    virtual public void OnPointerUp(PointerEventData e)
    {
        onUp?.Invoke();
        OnUpEvt?.Invoke();
    }

    virtual public void OnBeginDrag(PointerEventData e)
    {
        onBeginDrag?.Invoke();
        OnBeginDragEvt?.Invoke();
    }

    virtual public void OnDrag(PointerEventData e)
    {
        onDrag?.Invoke();
        OnDragEvt?.Invoke();
    }

    virtual public void OnEndDrag(PointerEventData e)
    {
        onEndDrag?.Invoke();
        OnEndDragEvt?.Invoke();
    }
    virtual public void OnPointerClick(PointerEventData e)
    {
        onClick?.Invoke();
        OnClickEvt?.Invoke();
        //Debug.Log("Button Clicked");
    }
}
