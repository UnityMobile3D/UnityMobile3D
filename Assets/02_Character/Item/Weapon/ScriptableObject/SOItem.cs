using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "SO/Item/ITem")]
public class SOItem : ScriptableObject
{
    public int ItemID;

    public uint Level;

    public float cooldown = 1.0f;

    [SerializeField] private Values m_tBaseValues;
    public Values BaseValues => m_tBaseValues;

    public SOItemEffect[] EquippedEffects;
    public SOItemEffect[] ReleaseEffects;
    public SOItemEffect[] UsingEffects;

    //장착하거나 월드에 떨어질 때 사용될 오브젝트
    public AssetReferenceGameObject ItemObject;

}
