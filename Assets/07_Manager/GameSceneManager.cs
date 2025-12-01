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

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager m_Instance;

    SceneInstance? m_tCurScene = null;
    [SerializeField] private List<SOSceneLoadData> m_listSceneLoad = new List<SOSceneLoadData>();

    private readonly Dictionary<string, AsyncOperationHandle<IList<Object>>> m_hashLabelValue
       = new Dictionary<string, AsyncOperationHandle<IList<Object>>>();

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


    public async Task LoadScene(string _strSceneName, CancellationToken _tCT = default)
    {
       
        SOSceneLoadData pSceneLoadData = null;
        for(int i = 0; i<m_listSceneLoad.Count; ++i)
        {
            if(m_listSceneLoad[i].scenename == _strSceneName)
            {
                pSceneLoadData = m_listSceneLoad[i];
                break;
            }
        }

        if (pSceneLoadData == null)
            return;

        //이전 씬 해제
        await UnLoadScene();
        
        //한 프레임 동안 처리할 시간
        List<Task> listTask = new List<Task>();
        listTask.Add(LoadLabel(pSceneLoadData.labelname,_tCT));

        for (int i = 0; i < pSceneLoadData.poolentries.Count; ++i)
            listTask.Add(ObjectPoolManager.m_Instance.LoadObject(pSceneLoadData.poolentries[i]));
      
        await Task.WhenAll(listTask);

        var pResultHandle = Addressables.LoadSceneAsync(_strSceneName);
        m_tCurScene = await pResultHandle.Task;
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
            var tSceneInst = m_tCurScene.Value;
            //이전씬을 이렇게 언 로드하면 뭐가 삭제되는지
            await Addressables.UnloadSceneAsync(tSceneInst).Task;
            m_tCurScene = null;
        }

        foreach(var tHandle  in m_hashLabelValue)
        {
            if (tHandle.Value.IsValid())
                Addressables.Release(tHandle.Value);
        }
        m_hashLabelValue.Clear();

        await Task.Yield(); //한 프레임 양보
    }

}
