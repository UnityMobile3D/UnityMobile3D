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
   

    //public static void UsingItem(SOItem _pSOItem, EffectContext _pCtx, List<AdditionalEffect> _arrAddtion)
    //{
    //     for (int i = 0; i < _arrAddtion.Count; ++i)
    //     {
    //         switch (_arrAddtion[i].Type)
    //         {
    //             case EffectAdditionType.Int:
    //                _pCtx. = UtilityMath.ApplyToValue<int, IntOps>(_pCtx.Value.Int, _arrAddtion[i].Value.Int, _arrAddtion[i].Operation);
    //                 break;
    //             case EffectAdditionType.Float:
    //                _pCtx.Float = UtilityMath.ApplyToValue<float, FloatOps>(_pCtx.Value.Float, _arrAddtion[i].Value.Float, _arrAddtion[i].Operation);
    //                 break;
    //             case EffectAdditionType.Vector4:
    //                 pCtx.Value.Vector4 = UtilityMath.ApplyToValue<Vector4, Vec4Ops>(_pCtx.Value.Vector4, _arrAddtion[i].Value.Vector4, _arrAddtion[i].Operation);
    //                 break;
    //         }
    //     }
        
    //     applay_effect(_pSOItem, pCtx);
    //}


    public static void ApplyEffectEquipped(SOItem _pSOItem, EffectContext _pCtx)
    {
        //foreach는 내부적으로 열거자(Enumerator) 를 만든 뒤 MoveNext()/Current로 도는 문법
        //열거자 객체가 힙에 만들어지면(참조형/박싱) → 임시 객체가 생기고 → 수집 대상이 되어 GC 스파이크
        for (int i = 0; i< _pSOItem.EquippedEffects.Length; ++i)
            _pSOItem.EquippedEffects[i].Apply(_pCtx);
    }

    public static void ApplyEffectRelease(SOItem _pSOItem, EffectContext _pCtx)
    {
        for (int i = 0; i < _pSOItem.ReleaseEffects.Length; ++i)
            _pSOItem.ReleaseEffects[i].Apply(_pCtx);
    }

    public static void ApplyEffectUsing(SOItem _pSOItem, EffectContext _pCtx)
    {
        for (int i = 0; i < _pSOItem.EquippedEffects.Length; ++i)
            _pSOItem.UsingEffects[i].Apply(_pCtx);
        
    }

};


