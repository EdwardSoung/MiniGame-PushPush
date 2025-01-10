using System;
using UnityEngine.SceneManagement;
using XUnityLibrary.Singleton;

public class SceneLoadManager : UnitySingleton<SceneLoadManager>
{
    public void LoadSceneAsync(string sceneName, LoadSceneMode mode, Action onComplete)
    {
        if(mode == LoadSceneMode.Single)
        {
            //�ε� ���� UI ����
            UIManager.Instance.ReleaseUI();
        }
        var handle = SceneManager.LoadSceneAsync(sceneName, mode);
        handle.completed += complete =>
        {
            onComplete?.Invoke();
        };
    }

    /// <summary>
    /// Load Scene with loading UI
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="onComplete"></param>
    /// <param name="loadingUI"></param>
    public void LoadSceneAsync(string sceneName, Action onComplete, E_UI_TYPE loadingUI)
    {
        UIManager.Instance.OpenLoadingUI(loadingUI);
        UIManager.Instance.ReleaseUI();

        var handle = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        handle.completed += complete =>
        {
            onComplete?.Invoke();
        };
    }
}
