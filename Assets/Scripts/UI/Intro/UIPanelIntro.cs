using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIPanelIntro : UIBase
{
    [SerializeField] private Button _startButton;

    public void Start()
    {
        _startButton.AddButtonListener(OnClickStart);
    }

    public void OnClickStart()
    {
        UIManager.Instance.OpenLoadingUI(E_UI_TYPE.UIPanelLoadingIntro);
        EventBus.Instance.Publish(new EventLoadAssets());
    }
}
