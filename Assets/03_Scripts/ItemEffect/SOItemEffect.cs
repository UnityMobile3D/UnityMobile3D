using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Values
{
    public int Int;
    public float Float;
    public Vector4 Vector4;
}

public struct EffectContext
{
    public GameObject pTarget;
    public GameObject pOwner;

    public Values Value;
}


public abstract class SOItemEffect : ScriptableObject
{
    public abstract void Apply(ref EffectContext _tEffectCnt);
}


