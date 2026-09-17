using Cysharp.Threading.Tasks;
using FirstVillain.Entities;
using System;
using System.Collections;
using UnityEngine;
using XSystem.EventBus;

public class IntroSceneController : MonoBehaviour
{
    private int _loadedTableCount = 0;

    private NetworkResponseHandler responseHandler = new();

    private void Start()
    {
        UIManager.Instance.OpenUI(E_UI_TYPE.UIPanelIntro);
        EventBus.Instance.Subscribe<EventLoadAssets>(OnStartLoadAssets);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<EventLoadAssets>(OnStartLoadAssets);
    }

    private void OnStartLoadAssets(EventLoadAssets e)
    {
        LoadTable();

        //음..로드하고 

        StartCoroutine(LoadingGauge());
    }

    private IEnumerator LoadingGauge()
    {
        //개수 어찌할지 고민...
        float maxGauge = 1;
        while (_loadedTableCount < maxGauge)
        {
            EventBus.Instance.Publish(new EventUpdateTableLadingProgress(_loadedTableCount / maxGauge));
            yield return null;
        }

        EventBus.Instance.Publish(new EventUpdateTableLadingProgress(1));
        SceneLoadManager.Instance.LoadSceneAsync("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single, OnCompleteLoad);
    }
    public void OnCompleteLoad()
    {
        EventBus.Instance.Publish(new EventStartLobby());
    }

    private void LoadTable()
    {
        _loadedTableCount++;
        var data = TableManager.Instance.LoadTable<JPlayerData>(E_TABLE.JPlayer);
        GameManager.Instance.SetPlayerData(data.list);
    }

    #region Sever

    public void StartLogin()
    {
        //ui에서 또는 뭐..playerpref로 userId 가져왔다고 치고..
        

        LoginAsync("testId", responseHandler.OnLogin).Forget();
    }

    public void LoadServerData()
    {
        var queue = new NetworkLoadQueue();

        //로그인 시 서버에서 호출할 API들 추가

        //queue.ExecuteAll()
    }

    public async UniTask LoginAsync(string userId, Action<ResLogin> OnLoginSuccess)
    {
        var res = await NetworkManager.Instance.SendAsync<ReqLogin, ResLogin>(new LoginRequest());

        if(res.Status == 0)
        {
            OnLoginSuccess?.Invoke(res);
            LoadServerData();
        }

    }

    #endregion
}
