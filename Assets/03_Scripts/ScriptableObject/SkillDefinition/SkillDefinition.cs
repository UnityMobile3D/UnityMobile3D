using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Game.Common;


[CreateAssetMenu(fileName = "SkillDefinition", menuName = "SO/Skill", order = int.MaxValue)]
public class SOSKill : ScriptableObject
{
    [Header("Meta")]
    [SerializeField]        private MetaProfile meta;
    [Header("Damage")]
    [SerializeField]        private DamageProfile damage;
    [Header("Defense")]
    [SerializeField]        private DefenseProfile defense;
    [Header("Buff")]
    [SerializeField]        private BuffProfile buff;
    [Header("Option")]
    [SerializeField]        private SkillOptionProfile option;
    [Header("Logic")]
    [SerializeField]        private LogicProfile logic;
    [Header("Animation")]
    [SerializeField]        private SKillAnimation animation;

    public MetaProfile Meta => meta;
    public DamageProfile Damage => damage;
    public DefenseProfile Defense => defense;
    public BuffProfile Buff => buff;
    public SkillOptionProfile Option => option;
    public LogicProfile Loggic => logic;
    public SKillAnimation Animation => animation;
}
