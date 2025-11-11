using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Load/SceneLoad")]
public class SOSceneLoadData : ScriptableObject
{
    public string scenename;
    public string labelname;
    public List<SOPoolEntry> poolentries;      
    public List<AssetReference> extraassets;   //사운드, 메테리얼 등,,
}