using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMonster : Monster
{
    //랜덤위치
    [SerializeField] private Transform m_pSpawnTr = null;
    [SerializeField] private float m_fYSpawnDiff = 10.0f;
    [SerializeField] private float m_fSpawnRadius = 3.0f;

    override protected void Awake()
    {
        base.Awake();
    }

    override protected void Update()
    {
        base.Update();
    }


    protected override void OnTriggerEnter(Collider other)
    {
        
    }

    public void SpawnObject()
    {
        Vector3 vBasePos = m_pSpawnTr.position;
    
        // xz 평면에서 원형으로 랜덤 위치
        Vector2 vRand = Random.insideUnitCircle * m_fSpawnRadius;

        Vector3 vSpawnPos = new Vector3
        (
            vBasePos.x + vRand.x,
            vBasePos.y ,
            vBasePos.z + vRand.y
        );

        Spawn(vSpawnPos);
    }

}
