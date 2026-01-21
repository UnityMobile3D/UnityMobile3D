using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Values
{
    public int Int;
    public float Float;
    public Vector4 Vector4;
}

[Serializable]
public class EffectContext
{
    public GameObject pTarget;
    public GameObject pOwner;

    public Values Value;
}


public abstract class SOItemEffect : ScriptableObject
{
    public abstract void Apply(EffectContext _tEffectCnt);

}


