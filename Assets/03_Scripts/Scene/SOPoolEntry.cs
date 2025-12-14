using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Load/PoolEntry")]
public class SOPoolEntry : ScriptableObject
{
    public ePoolType type;
    public AssetReferenceGameObject prefabRef;
    public int preload = 8;
    public int max = 12;
}