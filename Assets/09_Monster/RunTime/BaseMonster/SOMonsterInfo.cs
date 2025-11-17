using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSkillInfo
{

    [Header("Attack Options")]
    public float CoolTime = 0.0f;
    public float AttackRange = 0.0f;
    public float AttackDamage = 0.0f;
    public float AttackPower = 0.0f;
    public bool isDown = false;

    [Header("Object Options")]
    public float LifeTime = 0.0f;
    public float AttackTime = 0.0f;
    public float MoveSpeed = 0.0f;

    [Header("Spawn Options")]
    public bool SpawnPlayerPos = false;
    public bool SpawnCurPos = false;
    public bool PlayerDir = false;
    public float SpawnDiff = 1.0f;

    public Vector3 AttackDir = Vector3.zero;
    public Vector3 SpawnPos = Vector3.zero;
    public Vector3 SpawnRot = Vector3.zero;

    public SOPoolEntry SOPoolEntry = null;

    public SpawnOption EndFrameSpawn = null;
}

[Serializable]
public class SpawnOption
{
    public SOPoolEntry SpawnObject;
    public Vector3 Offset;
}


[CreateAssetMenu(menuName = "SO/Monster/MonsterInfo")]
public class SOMonsterInfo : ScriptableObject
{
    public List<MonsterSkillInfo> skillinfo = new List<MonsterSkillInfo>();
}
