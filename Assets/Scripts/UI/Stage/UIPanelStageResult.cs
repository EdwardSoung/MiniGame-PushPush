using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XUnityLibrary.EventBus;
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

    private List<KeyValuePair<int, int>> _propResults = new ();
    private Dictionary<int, JPropInfoData> _propInfoDict = new ();

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

        _propInfoDict = TableManager.Instance.GetPropInfoList().ToDictionary(x => x.Id);

        foreach (var prop in propDict)
        {
            //var table = TableManager.Instance.GetPropInfoById(prop.Key);

            _pointItemScrollView.Add(_pointItemScrollView.GetPrefabBoundSize);
            
            //var obj = AddressableManager.Instance.Spawn(table.UIPrefabName, _pointItemRoot);
            //var item = obj.GetComponent<PointItem>();
            //item.transform.localScale = Vector3.one;
            totalPoint += _propInfoDict[prop.Key].Point * prop.Value;
            //item.SetData(table, prop.Value);
        }

        _totalPointText.SetText(totalPoint.ToString());

        _pointItemScrollView.Refresh();

        //TODO 먹은 코인, 보석 표기해야함
        //획득 리스트에 맞게 생성
        //AddressableManager.Instance.Spawn("RewardItem", _rewardRoot)
    }

    private void OnScrollViewUpdate(int index, Cell_Base baseCell)
    {
        if(baseCell is PointItem cell)
        {
            var result = _propResults[index];

            var table = _propInfoDict[result.Key];

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
