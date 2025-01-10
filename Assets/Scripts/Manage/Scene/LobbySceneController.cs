using XUnityLibrary.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneController : MonoBehaviour
{
    private void Awake()
    {
        EventBus.Instance.Subscribe<EventStartLobby>(OnStartLobby);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<EventStartLobby>(OnStartLobby);
    }

    private void OnStartLobby(EventStartLobby e)
    {
        UIManager.Instance.ResetUICam();
        //·Îºñ UI Open
        UIManager.Instance.OpenUI(E_UI_TYPE.UIPanelLobby);
        UIManager.Instance.CloseLoadingUI();
    }

}
