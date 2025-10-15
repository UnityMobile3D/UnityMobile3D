using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UIData/Catalog/Entry UI", fileName = "SOEntryUI")]
public class SOEntryUI : ScriptableObject
{
    //비트로도 가능
    public enum eUIType
    {
        None,
        Skill,
        Item,
    }


    [SerializeField] private int id;                    // 고정 키
    [SerializeField] private Sprite icon;               // 아이콘
    [SerializeField] private eUIType type;
    [SerializeField] private int sortKey = 0;           // 정렬 우선순위(작을수록 앞)
    [SerializeField] private float cooldown;            // 쿨타임

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