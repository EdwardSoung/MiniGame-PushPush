using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using XUnityLibrary.EventBus;
using XUnityLibrary.Singleton;

public class UIManager : UnitySingleton<UIManager>
{
    [SerializeField] private Canvas _uiCanvas;
    private Dictionary<E_UI_ROOT_TYPE, Transform> _uiRootDict = new();
    private Dictionary<E_UI_TYPE, GameObject> _loadedUIDict = new();

    //Basic UI
    private Stack<UIBase> _uiStack = new();

    //Loading UI - Loading은 2개가 나오지 않을거라 생각되어 1개로
    private UIBase _loadingUI;
    public UIBase LoadingUI { get { return _loadingUI; } }

    //SystemUI
    private List<UIBase> _systemUIList = new();

    protected override void AwakeSingleton()
    {
        base.AwakeSingleton();
        InitRoot();
    }
    
    private void InitRoot()
    {
        for (int i = 0; i < _uiCanvas.transform.childCount; i++)
        {
            var root = _uiCanvas.transform.GetChild(i);
            //GetChild가 순차로 가져올 것 같긴 한데 혹시 몰라서 체크 후 대입
            foreach (E_UI_ROOT_TYPE rootType in System.Enum.GetValues(typeof(E_UI_ROOT_TYPE)))
            {
                if (rootType.ToString() == root.name)
                {
                    _uiRootDict.Add(rootType, root);
                }
            }
        }
    }

    #region Open UI
    public UIBase OpenUI(E_UI_TYPE type)
    {
        if(_uiStack.Count > 0)
        {
            _uiStack.Peek().gameObject.SetActive(false);
        }
        if(_loadedUIDict.ContainsKey(type))
        {
            return GenerateUI(_loadedUIDict[type], E_UI_ROOT_TYPE.UI);
        }
        else
        {
            var obj = AddressableManager.Instance.LoadAssetAsync<GameObject>(type.ToString());
            
            _loadedUIDict.Add(type, obj);
            return GenerateUI(obj, E_UI_ROOT_TYPE.UI);
        }
    }

    //시스템 메시지, 토스트 메시지 - 최상단
    public UIBase OpenSystemUI(E_UI_TYPE type)
    {
        if (_loadedUIDict.ContainsKey(type))
        {
            return GenerateUI(_loadedUIDict[type], E_UI_ROOT_TYPE.System);
        }
        else
        {
            var obj = AddressableManager.Instance.LoadAssetAsync<GameObject>(type.ToString());
            _loadedUIDict.Add(type, obj);
            return GenerateUI(obj, E_UI_ROOT_TYPE.System);
        }
    }

    public UIBase OpenLoadingUI(E_UI_TYPE type)
    {
        if (_loadedUIDict.ContainsKey(type))
        {
            return GenerateUI(_loadedUIDict[type], E_UI_ROOT_TYPE.Loading);
        }
        else
        {
            var obj = AddressableManager.Instance.LoadAssetAsync<GameObject>(type.ToString());
            _loadedUIDict.Add(type, obj);
            return GenerateUI(obj, E_UI_ROOT_TYPE.Loading);
        }
    }

    #endregion OpenUI

    private UIBase GenerateUI(GameObject uiObj, E_UI_ROOT_TYPE rootType)
    {
        var obj = Instantiate(uiObj, _uiRootDict[rootType]);
        var uiBase = obj.GetComponent<UIBase>();
        uiBase.SetUIBaseType(rootType);

        switch (rootType)
        {
            case E_UI_ROOT_TYPE.UI:
                _uiStack.Push(uiBase);
                break;
            case E_UI_ROOT_TYPE.Loading:
                _loadingUI = uiBase;
                break;
            case E_UI_ROOT_TYPE.System:
                _systemUIList.Add(uiBase);
                break;
        }

        uiBase.Open();
        return uiBase;
    }

    #region Close UI
    public UIBase CloseUI()
    {
        if (_uiStack.Count > 1)
        {
            var targetUI = _uiStack.Pop();
            targetUI.CloseAction();
            Destroy(targetUI.gameObject);
        }
        
        var prevUI = _uiStack.Peek();
        prevUI.gameObject.SetActive(true);
        return prevUI;

    }

    public void CloseSystemUI(UIBase ui)
    {
        _systemUIList.Remove(ui);
        Destroy(ui.gameObject);
    }

    public void CloseLoadingUI()
    {
        if(_loadingUI == null)
        {
            return;
        }
        //StartCoroutine(DelayCloseLoading());
        _loadingUI.CloseAction();
        Destroy(_loadingUI.gameObject);
        _loadingUI = null;
    }

    public void ReleaseUI()
    {
        while(_uiStack.Count > 0)
        {
            var ui = _uiStack.Pop();
            ui.CloseAction();
            Destroy(ui.gameObject);
        }

        EventBus.Instance.Publish(new EventStartLobby());

        foreach (var systemUI in _systemUIList)
        {
            Destroy(systemUI.gameObject);
        }
        _systemUIList = new();
    }

    #endregion Close UI

    #region Utils
    public Camera GetUICam()
    {
        var uiCam = _uiCanvas.worldCamera;
        uiCam.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
        return uiCam;
    }

    public void ResetUICam()
    {
        var uiCam = _uiCanvas.worldCamera;
        uiCam.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
    }
    #endregion Utils
}
