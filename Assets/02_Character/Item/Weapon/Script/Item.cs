using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private EffectContext m_tEffectValue;
    public EffectContext EffectContext => m_tEffectValue;

    [SerializeField] private SOItem m_pItem;
    public SOItem GetItem() => m_pItem;
}
