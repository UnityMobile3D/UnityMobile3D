using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : BaseUI, IContainer
{

    [SerializeField] private ButtonUI m_pCloseButton;
    [SerializeField] private ButtonUI m_pSeleteButton;
    [SerializeField] private Container m_pItemContainer;

    [SerializeField] private eContainerType m_eContainerType = eContainerType.Store; // �� �ν����Ϳ� ��Ӵٿ����� ����
    public eContainerType ContainerType { get => m_eContainerType; }

    protected override void Awake()
    {
        base.Awake();

        //close selete �Լ� ���ε�
        //m_pCloseButton.OnUpEvt += close_tap;

        m_pItemContainer.OnSelectEvt += select_shapitem;

    }

    private void Update()
    {

    }

    private void OnDisable()
    {
        //m_pPlayerInterface.
    }
    private void close_tap()
    {
        //���̱��� �����ϱ� ���ؼ�
        gameObject.SetActive(false);

    }
    private void select_shapitem()
    {
        SlotView pTarget = m_pItemContainer.GetTargetSlot();
        SOShapItem pShapItem = pTarget.SOEntryUI as SOShapItem;

        if (pShapItem != null)
        {
           //������ �Ŵ������� �÷��̾� ���� �� �������� �����Դٸ� �� �� DataService�� ���ؼ� ����
        }
    }

    //IContainer ����
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0) { return null; }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0) { return -1; }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0) { return -1; }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0) { return false; }
    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0) { return false; }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        //���� ��ųâ���� ��ų�� �������� ���� ����
        m_pItemContainer.ClearTarget();
        return true;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx) { return false; }


}
