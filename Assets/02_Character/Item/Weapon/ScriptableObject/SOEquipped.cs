using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item/Effect/Equipped")]


public class SOEquipped : SOItemEffect
{
    public eEquipType EquipType;
    public override void Apply(EffectContext _tEffectCnt)
    {
        Player pPlayer = GameManager.m_Instance.Player;
        //여기서 해당 위체 맞는 얘를 Target으로 잡기 (팔, 다리 ..)
        
        if (_tEffectCnt.pOwner == null || pPlayer == null)
            return;

        Transform pTransform = pPlayer.GetEquipPoint(EquipType);
        _tEffectCnt.pOwner.transform.SetParent(pTransform, false);
    }
}
