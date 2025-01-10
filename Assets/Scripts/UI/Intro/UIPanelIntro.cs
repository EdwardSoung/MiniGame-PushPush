using UnityEngine;
using UnityEngine.UI;
using XUnityLibrary.EventBus;

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
