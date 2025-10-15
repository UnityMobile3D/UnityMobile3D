using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//추가할 타입의 값
public enum EffectAdditionType
{
    None,
    Int,
    Float,
    Vector4,
}

//추가 효과 계수
public struct AdditionalEffect
{
    public EffectAdditionType Type;
    public EffectOperation Operation;
    public Values Value;
}

public class ItemEffectRunner : MonoBehaviour
{
    static public void UsingItem(SOItemUI _pSOItemUI, GameObject _pOwner, GameObject _pTarget)
    {
        EffectContext pEffectContext = create_context(_pSOItemUI, _pOwner, _pTarget);
        applay_effect(_pSOItemUI, ref pEffectContext);
    }

    public static void UsingItem(SOItemUI _pSOItemUI, GameObject _pOwner, GameObject _pTarget, List<AdditionalEffect> _arrAddtion)
    {
        EffectContext pCtx = create_context(_pSOItemUI, _pOwner, _pTarget);
       
         for (int i = 0; i < _arrAddtion.Count; ++i)
         {
             switch (_arrAddtion[i].Type)
             {
                 case EffectAdditionType.Int:
                     pCtx.Value.Int = UtilityMath.ApplyToValue<int, IntOps>(pCtx.Value.Int, _arrAddtion[i].Value.Int, _arrAddtion[i].Operation);
                     break;
                 case EffectAdditionType.Float:
                     pCtx.Value.Float = UtilityMath.ApplyToValue<float, FloatOps>(pCtx.Value.Float, _arrAddtion[i].Value.Float, _arrAddtion[i].Operation);
                     break;
                 case EffectAdditionType.Vector4:
                     pCtx.Value.Vector4 = UtilityMath.ApplyToValue<Vector4, Vec4Ops>(pCtx.Value.Vector4, _arrAddtion[i].Value.Vector4, _arrAddtion[i].Operation);
                     break;
             }
         }
        
         applay_effect(_pSOItemUI, ref pCtx);
    }

    static private EffectContext create_context(SOItemUI _SOItemUI, GameObject _pOwner = null, GameObject _pTarget = null)
    {
        EffectContext pCtx = new EffectContext();
        pCtx.pTarget = _pTarget;
        pCtx.pOwner = _pOwner;
        pCtx.Value = _SOItemUI.BaseValues;

        return pCtx;
    }

    static void applay_effect(SOItemUI _pSOItemUI, ref EffectContext _pCtx)
    {
        //foreach는 내부적으로 열거자(Enumerator) 를 만든 뒤 MoveNext()/Current로 도는 문법
        //열거자 객체가 힙에 만들어지면(참조형/박싱) → 임시 객체가 생기고 → 수집 대상이 되어 GC 스파이크
        for (int i = 0; i< _pSOItemUI.Effects.Length; ++i)
        {
            _pSOItemUI.Effects[i].Apply(ref _pCtx);
        }
    }

};


