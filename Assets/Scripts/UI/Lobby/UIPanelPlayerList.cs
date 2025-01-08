using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelPlayerList : UIBase
{
    [SerializeField] private Transform _playerRoot;

    [SerializeField] private Button _selectButton;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _homeButton;

    [SerializeField] private Slider _strSlider;
    [SerializeField] private Slider _rangeSlider;
    [SerializeField] private Slider _speedSlider;

    [SerializeField] private TextMeshProUGUI _strText;
    [SerializeField] private TextMeshProUGUI _rangeText;
    [SerializeField] private TextMeshProUGUI _speedText;

    private PlayerInfo _curPlayerInfo;
    private int _curPlayerIdx;
    private GameObject _curPlayerPrefab;

    public override void Open()
    {
        _curPlayerInfo = PlayerPrefsManager.LoadMainPlayer();

        _curPlayerIdx = GameManager.Instance.GetPlayerDataIndex(_curPlayerInfo);

        _selectButton.AddButtonListener(OnClickSelectPlayer);
        _prevButton.AddButtonListener(OnClickPrevButton);
        _nextButton.AddButtonListener(OnClickNextButton);
        _backButton.AddButtonListener(OnClickBack);
        _homeButton.AddButtonListener(OnClickHome);

        UpdateData();

        base.Open();
    }

    private void UpdateData()
    {
        LoadPlayerPrefab();

        _strSlider.value = _curPlayerInfo.STR / (float)Constants.STR_MAX;
        _rangeSlider.value = _curPlayerInfo.RANGE / Constants.RANGE_MAX;
        _speedSlider.value = _curPlayerInfo.SPEED / Constants.SPEED_MAX;

        _strText.SetText(_curPlayerInfo.STR.ToString());
        _rangeText.SetText(((int)_curPlayerInfo.RANGE * 10).ToString());
        _speedText.SetText(((int)_curPlayerInfo.SPEED * 10).ToString());
    }

    private void LoadPlayerPrefab()
    {
        if(_curPlayerPrefab != null)
        {
            AddressableManager.Instance.Release(_curPlayerPrefab);
        }
        _curPlayerPrefab = AddressableManager.Instance.Spawn(_curPlayerInfo.UIPrefabName, _playerRoot);
        _curPlayerPrefab.ResetTransform();
        _curPlayerPrefab.transform.localPosition = Vector3.zero;
    }
    #region OnClick
    private void OnClickPrevButton()
    {
        if(--_curPlayerIdx < 0)
        {
            _curPlayerIdx += GameManager.Instance.PlayerCount;
        }

        _curPlayerInfo = GameManager.Instance.GetPlayerDataByIndex(_curPlayerIdx);

        UpdateData();
    }

    private void OnClickNextButton()
    {
        if(++_curPlayerIdx >= GameManager.Instance.PlayerCount)
        {
            _curPlayerIdx -= GameManager.Instance.PlayerCount;
        }

        _curPlayerInfo = GameManager.Instance.GetPlayerDataByIndex(_curPlayerIdx);

        UpdateData();
    }

    private void OnClickSelectPlayer()
    {
        PlayerPrefsManager.SaveMainPlayer(_curPlayerInfo.Id);
    }

    private void OnClickBack()
    {
        UIManager.Instance.CloseUI();
    }

    private void OnClickHome()
    {
        UIManager.Instance.ReleaseUI();
    }
    #endregion OnClick

    public override void CloseAction()
    {
        if(_curPlayerPrefab != null)
        {
            AddressableManager.Instance.Release(_curPlayerPrefab);
        }
        base.CloseAction();
    }
}
