using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XUnityLibrary.EventBus;

public class UIPanelStageResult : UIBase
{
    [SerializeField] private Transform _pointItemRoot;
    [SerializeField] private Transform _rewardRoot;

    [SerializeField] private Button _confirmButton;

    [SerializeField] private TextMeshProUGUI _totalPointText;

    public override void Open()
    {
        _confirmButton.AddButtonListener(OnClickConfirm);
        base.Open();
    }

    public void SetData(Dictionary<int, int> propDict)
    {
        int totalPoint = 0;
        foreach (var prop in propDict)
        {
            var table = TableManager.Instance.GetPropInfoById(prop.Key);
            
            var obj = AddressableManager.Instance.Spawn(table.UIPrefabName, _pointItemRoot);
            var item = obj.GetComponent<PointItem>();
            item.transform.localScale = Vector3.one;
            totalPoint += table.Point * prop.Value;
            item.SetData(table, prop.Value);
        }

        _totalPointText.SetText(totalPoint.ToString());

        //TODO 먹은 코인, 보석 표기해야함
        //획득 리스트에 맞게 생성
        //AddressableManager.Instance.Spawn("RewardItem", _rewardRoot)
    }

    private void OnClickConfirm()
    {
        StageManager.Instance.GameEnd();
        SceneLoadManager.Instance.LoadSceneAsync("Lobby", OnLoadSceneComplete, E_UI_TYPE.UIPanelLoading);
    }

    private void OnLoadSceneComplete()
    {
        AddressableManager.Instance.ReleaseAll();
        EventBus.Instance.Publish(new EventStartLobby());
    }
}
