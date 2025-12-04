using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterSkillInfo
{
    public MonsterSkillOption SkillOption = null;
    public MonsterSpawnOption SpawnOption = null;
}


[CreateAssetMenu(menuName = "SO/Monster/MonsterInfo")]
public class SOMonsterInfo : ScriptableObject
{
    public List<MonsterSkillInfo> skillinfo = new List<MonsterSkillInfo>();
}
