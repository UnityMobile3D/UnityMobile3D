using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[CreateAssetMenu(menuName = "UIData/Catalog/Entry UI", fileName = "SOEntryUI")]
public class SOEntryUI : ScriptableObject
{
    //비트로도 가능
    [System.Serializable]
    public enum eUIType
    {
        None = 0,
        Skill = 8,
        Item = 16,
        Equip = 24,
    }

    [SerializeField] LocalizedString localizedName = null;
    [SerializeField] private int id;                    // 고정 키
    [SerializeField] protected Sprite icon;             // 아이콘
    [SerializeField] private eUIType type;

    private string name = "";

    protected uint hashCode = (uint)eUIType.None;

    public string Name => name;
    public int Id => id;
    public Sprite Icon => icon;
    public eUIType Type => type;


    private void OnEnable()
    {
        if (localizedName == null)
            return;

        localizedName.StringChanged += OnNameChanged;
    }

    private void OnDisable()
    {
        if (localizedName == null)
            return;

        localizedName.StringChanged -= OnNameChanged;
    }

    public virtual uint GetUIHashCode()
    {
        uint iHashCode = (uint)type;
        return iHashCode;
    }

    public uint GetUITypeCode()
    {
        uint iHashCode = (uint)type;
        return iHashCode;
    }

    private void OnNameChanged(string value)
    {
        name = value;
    }
}