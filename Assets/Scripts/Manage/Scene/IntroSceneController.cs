using FirstVillain.Entities;
using System.Collections;
using UnityEngine;
using XUnityLibrary.EventBus;

public class IntroSceneController : MonoBehaviour
{
    private int _loadedTableCount = 0;

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
        StartCoroutine(LoadingGauge());
    }

    private IEnumerator LoadingGauge()
    {
        //°³¼ö ¾îÂîÇÒÁö °í¹Î...
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
}
