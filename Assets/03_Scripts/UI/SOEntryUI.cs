using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UIData/Catalog/Entry UI", fileName = "SOEntryUI")]
public class SOEntryUI : ScriptableObject
{
    //��Ʈ�ε� ����
    public enum eUIType
    {
        None,
        Skill,
        Item,
    }


    [SerializeField] private int id;                 // ���� Ű(���� �Է� ����)
    [SerializeField] private Sprite icon;               // ������(��Ʋ�� ����)
    [SerializeField] private eUIType type;
    [SerializeField] private int sortKey = 0;           // ���� �켱����(�������� ��)
    [SerializeField] private float cooldown;

    protected uint hashCode = (uint)eUIType.None;

    public int Id => id;
    public Sprite Icon => icon;
    public eUIType Type => type;
    public int SortKey => sortKey;

    public float Cooldown => cooldown;

    public virtual uint GetUIHashCode()
    {
        uint iHashCode = (uint)type;
        return iHashCode;
    }
}