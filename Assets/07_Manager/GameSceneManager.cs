using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager m_Instance;

    SceneInstance? m_tCurScene = null;
    
    [SerializeField] private List<SOSceneLoadData> m_listSceneLoad = new List<SOSceneLoadData>();
    [SerializeField] private LoadingOverlay m_pLoadingOverlay = null;
    private readonly Dictionary<string, AsyncOperationHandle<IList<Object>>> m_hashLabelValue = new();
    private SemaphoreSlim m_pSemaphore = new SemaphoreSlim(4, 4);


    private float m_fLoadRatio = 0.0f;

    private void Awake()
    {
        if (m_Instance != null)
            Destroy(gameObject);

        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        Addressables.InitializeAsync();
    }


    public async Task LoadScene(SOPortal _pNextScenePortal, CancellationToken _tCT = default)
    {
        m_pLoadingOverlay.ShowLoadingImage();
        
        SOSceneLoadData pSceneLoadData = null;
        for(int i = 0; i<m_listSceneLoad.Count; ++i)
        {
            if(m_listSceneLoad[i].CurrentScene.AssetGUID == _pNextScenePortal.NextScene.AssetGUID)
            {
                pSceneLoadData = m_listSceneLoad[i];
                break;
            }
        }

        if (pSceneLoadData == null)
            return;

        //이전 씬 오브젝트 해제
        await UnLoadScene();
        m_pLoadingOverlay.SetProgress(0.2f);

        //한 프레임 동안 처리할 시간
        List<Task> listTask = new List<Task>();
        for(int i = 0; i<pSceneLoadData.labelnames.Count; ++i)
            listTask.Add(LoadLabel(pSceneLoadData.labelnames[i], _tCT));

        for (int i = 0; i < pSceneLoadData.poolentries.Count; ++i)
            listTask.Add(ObjectPoolManager.m_Instance.LoadObject(pSceneLoadData.poolentries[i]));

        System.GC.Collect();
        while (Task.WhenAll(listTask).IsCompleted == false)
        {
            m_pLoadingOverlay.SetProgress(0.5f); 
            await Task.Yield();
        }
        //await Task.WhenAll(listTask);

        //single로 한다면 자동으로 이전 씬 해제
        var pResultHandle = _pNextScenePortal.NextScene.LoadSceneAsync(LoadSceneMode.Single);
        while (pResultHandle.IsDone == false)
        {
            m_pLoadingOverlay.SetProgress(0.5f + pResultHandle.PercentComplete * 0.5f);
            await Task.Yield();
        }
        m_tCurScene = pResultHandle.Result;
        //m_tCurScene = await pResultHandle.Task;


        FindPortal(_pNextScenePortal.ePortalID);

        m_pLoadingOverlay.CompletedLoading();
    }

    private async Task LoadLabel(string _strLabel, CancellationToken _tCT)
    {
        if (string.IsNullOrEmpty(_strLabel)) 
            return;
        if (m_hashLabelValue.ContainsKey(_strLabel)) 
            return;

        //동시접근 과부화 방지
        await m_pSemaphore.WaitAsync(_tCT);

        try
        {
            var tHandle = Addressables.LoadAssetsAsync<Object>(_strLabel, null);
            var listTask = await tHandle.Task; // 완료까지 논블로킹 대기
            m_hashLabelValue[_strLabel] = tHandle;
        }
        finally
        {
            m_pSemaphore.Release();
        }
    }

  
    public async Task UnLoadScene()
    {
        if(m_tCurScene != null)
        {
            //var tSceneInst = m_tCurScene.Value;

            //기존 씬은 Load할때 자동으로 언로드해줌
            //Addressables.UnloadSceneAsync(tSceneInst);
            m_tCurScene = null;
        }
        MonsterManager.m_Instance.ClearMonsters();

        foreach (var tHandle  in m_hashLabelValue)
        {
            if (tHandle.Value.IsValid())
                Addressables.Release(tHandle.Value);
        }
        m_hashLabelValue.Clear();

        ObjectPoolManager.m_Instance.DeleteObject(ePoolType.Stack);

        await Task.Yield(); //한 프레임 양보
    }

    private void FindPortal(ePortalID _eTargetPortalID)
    {
        Portal[] arrPortal = GameObject.FindObjectsOfType<Portal>();

        for (int i = 0; i < arrPortal.Length; ++i)
        {
            if (arrPortal[i].PortalID == _eTargetPortalID)
            {
                arrPortal[i].EnterPlayer();
                return;
            }
        }
    }

}
