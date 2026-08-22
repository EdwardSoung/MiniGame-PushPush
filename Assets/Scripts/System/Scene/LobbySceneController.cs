using XSystem.EventBus;
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
        //로비 UI Open
        UIManager.Instance.OpenUI(E_UI_TYPE.UIPanelLobby);
        UIManager.Instance.CloseLoadingUI();
    }

}
