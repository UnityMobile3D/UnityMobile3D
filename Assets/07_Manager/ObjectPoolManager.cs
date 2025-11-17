using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.EventSystems.EventTrigger;

public interface IPoolAble
{
    void OnSpawn();          // 상태 초기화(애니/파티클 재생 등)
    void OnDespawn();        // 정리(파티클 Stop&Clear, 물리값 리셋 등)


}
public class ObjectPoolManager : MonoBehaviour
{
    public class PoolBucket
    {
        public SOPoolEntry entry;                                     // 설정값 참조
        public GameObject prefab;
        public Queue<GameObject> pool = new Queue<GameObject>();      // 인스턴스 풀
    }

    private Dictionary<string, PoolBucket> m_hashPoolBucket = new Dictionary<string, PoolBucket>();
    [SerializeField] private List<SOPoolEntry> m_listFixed = new List<SOPoolEntry>();
    private readonly SemaphoreSlim m_pSemaphore = new SemaphoreSlim(4, 4); // 동시 Instantiate 
    public static ObjectPoolManager m_Instance { get; private set; }

    private async void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        var listTask = new List<Task>();
        for(int i = 0; i<m_listFixed.Count; i++)
            listTask.Add(LoadObject(m_listFixed[i]));

        await Task.WhenAll(listTask);
    }

    public async Task<PoolBucket> LoadObject(SOPoolEntry _pPoolEntry)
    {
        var prefabRef = _pPoolEntry.prefabRef;
        if (prefabRef == null)
            return null;

        if (m_hashPoolBucket.TryGetValue(prefabRef.AssetGUID, out var pPool) == true)
            return pPool;

        var pBucket  = new PoolBucket();
        pBucket.entry = _pPoolEntry;

        GameObject pPrefab = null;
        for(int i = 0; i<_pPoolEntry.preload; ++i)
        {
            await m_pSemaphore.WaitAsync();
            try
            {
                var tInsHandle = Addressables.InstantiateAsync(_pPoolEntry.prefabRef);
                var pGameObject = await tInsHandle.Task;
                pPrefab = pGameObject;

                pGameObject.SetActive(false);
                pBucket.pool.Enqueue(pGameObject);
            }
            finally
            {
                m_pSemaphore.Release();
            }
        }

        pBucket.prefab = pPrefab;
        m_hashPoolBucket[prefabRef.AssetGUID] = pBucket;
        return pBucket;
    }

    public void DeleteObject(string _strKey)
    {
        if (m_hashPoolBucket.ContainsKey(_strKey) == false)
            return;

        //오브젝트 제거
        var pBucket = m_hashPoolBucket[_strKey];
        while (pBucket.pool.Count > 0)
            Addressables.ReleaseInstance(pBucket.pool.Dequeue());

        m_hashPoolBucket.Remove(_strKey);
    }


    public GameObject GetObject(string _strKey , in Vector3 vPosition, in Vector3 vRot)
    {
        if (m_hashPoolBucket.TryGetValue(_strKey, out var pBucket) == false)
            return null;

        GameObject pObject = null;

        if (pBucket.pool.Count <= 0)
            pObject = GameObject.Instantiate(pBucket.prefab, vPosition, Quaternion.Euler(vRot));
        else
        {
            pObject = pBucket.pool.Dequeue();
            pObject.transform.SetPositionAndRotation(vPosition, Quaternion.Euler(vRot));
        }
        pObject.SetActive(true);

        if (pObject.TryGetComponent<IPoolAble>(out var IPoolCom) == true)
            IPoolCom.OnSpawn();

        return pObject;
    }

    public void PushObject(string _strKey, GameObject _pObject)
    {
        if (_pObject.TryGetComponent<IPoolAble>(out var IPoolCom) == true)
            IPoolCom.OnDespawn();

        if (m_hashPoolBucket.TryGetValue(_strKey, out var pBucket) == false)
        {
            Destroy(_pObject);
            return;
        }

        _pObject.SetActive(false);
        pBucket.pool.Enqueue(_pObject);


    }
}
