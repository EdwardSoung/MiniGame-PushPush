using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelLobby : UIBase
{
    [SerializeField] private Transform _characterRoot;

    [SerializeField] private Button _singleStageButton;
    [SerializeField] private Button _playerListButton;

    private PlayerInfo _curPlayerInfo;
    private GameObject _curPlayerPrefab;
    public override void Open()
    {
        InitButtons();

        UpdatePlayer();
        base.Open();

        _curPlayerPrefab.SetActive(true);
    }

    private void OnEnable()
    {
        if(_curPlayerInfo != GameManager.Instance.MainPlayer)
        {
            if (_curPlayerPrefab != null)
            {
                AddressableManager.Instance.Release(_curPlayerPrefab);
                UpdatePlayer();                 
                _curPlayerPrefab.SetActive(true);
            }
        }
        
    }

    private void UpdatePlayer()
    {
        _curPlayerInfo = GameManager.Instance.MainPlayer;
        _curPlayerPrefab = AddressableManager.Instance.Spawn(GameManager.Instance.MainPlayer.UIPrefabName, _characterRoot);
        _curPlayerPrefab.SetActive(false);
        _curPlayerPrefab.ResetTransform();
    }

    private void InitButtons()
    {
        _singleStageButton.AddButtonListener(OnClickSingleStage);
        _playerListButton.AddButtonListener(OnClickPlayerList);
    }

    #region OnClick

    private void OnClickSingleStage()
    {
        SceneLoadManager.Instance.LoadSceneAsync("Stage_Single", UnityEngine.SceneManagement.LoadSceneMode.Single, OnLoadSingleStageComplete);
    }

    private void OnClickPlayerList()
    {
        UIManager.Instance.OpenUI(E_UI_TYPE.UIPanelPlayerList);
    }

    #endregion OnClick

    private void OnLoadSingleStageComplete()
    {
        EventBus.Instance.Publish(new EventStartStage(GameManager.Instance.MainPlayer, E_STAGE_TYPE.Single_TimeAttack));
    }
}
