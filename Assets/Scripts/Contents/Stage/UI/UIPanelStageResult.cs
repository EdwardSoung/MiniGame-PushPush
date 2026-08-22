using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XSystem.EventBus;
using UniRx;
using System.Linq;
using FirstVillain.Entities;

public class UIPanelStageResult : UIBase
{
    [SerializeField] private Transform _pointItemRoot;
    [SerializeField] private InfiniteScrollView_V _pointItemScrollView;
    [SerializeField] private Transform _rewardRoot;

    [SerializeField] private Button _confirmButton;

    [SerializeField] private TextMeshProUGUI _totalPointText;

    [SerializeField] private RewardItem[] _rewardItems;

    private List<KeyValuePair<int, int>> _propResults = new ();

    private void Awake()
    {
        _pointItemScrollView.EventCellUpdateTrigger.Subscribe(data =>
        {
           OnScrollViewUpdate(data.Item1, data.Item2);
        }).AddTo(_disposable);
    }
    public override void Open()
    {
        _confirmButton.AddButtonListener(OnClickConfirm);
        base.Open();
    }

    public void SetData(Dictionary<int, int> propDict)
    {
        int totalPoint = 0;

        _propResults = propDict.ToList();

        foreach (var prop in propDict)
        {
            _pointItemScrollView.Add(_pointItemScrollView.GetPrefabBoundSize);
            
            totalPoint += DataManager.Instance.Prop.GetPropInfoById(prop.Key).Point * prop.Value;
        }

        _totalPointText.SetText(totalPoint.ToString());

        _pointItemScrollView.Refresh();

        var stageReward = DataManager.Instance.StageReward.GetStageReward(totalPoint);
        var rewards = DataManager.Instance.RewardGroup.GetRewardsByGroupId(stageReward.RewardGroup);

        foreach (var item in _rewardItems)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var reward in rewards)
        {
            _rewardItems[reward.Type].gameObject.SetActive(true);
            _rewardItems[reward.Type].SetData(reward, reward.Amount);
        }

        //TODO 먹은 코인, 보석 표기해야함
        //획득 리스트에 맞게 생성
        //AddressableManager.Instance.Spawn("RewardItem", _rewardRoot)
    }

    private void OnScrollViewUpdate(int index, Cell_Base baseCell)
    {
        if(baseCell is PointItem cell)
        {
            var result = _propResults[index];

            var table = DataManager.Instance.Prop.GetPropInfoById(result.Key);

            cell.SetData(table, result.Value);
        }
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
