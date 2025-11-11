using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSkillInfo
{
    public float CoolTime = 0.0f;

    public float AttackRange = 0.0f;

    public float AttackDamage = 0.0f;

    public float LifeTime = 0.0f;

    public float MoveSpeed = 0.0f;

    public bool SpawnCurPos = false;
    public bool PlayerDir = false;
    public bool RandomDir = false;
    public float SpawnDiff = 0.0f;

    public Vector3 AttackDir = Vector3.zero;
    public Vector3 SpawnPos = Vector3.zero;
    public Vector3 SpawnRot = Vector3.zero;
  
    public SOPoolEntry SOPoolEntry = null;
}

[CreateAssetMenu(menuName = "SO/Monster/MonsterInfo")]
public class SOMonsterInfo : ScriptableObject
{
    public List<MonsterSkillInfo> skillinfo = new List<MonsterSkillInfo>();
}
