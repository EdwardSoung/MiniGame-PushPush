using FirstVillain.Entities;
using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPos;
    
    [SerializeField] private List<BoxCollider> _propGenerateArea;
    [SerializeField] private List<Transform> _carGenPosList;
    [SerializeField] private List<Transform> _planeGenPosList;
    

    [SerializeField] private List<Transform> _itemSpawnPosList;

    private int _maxPropCount = 50;
    private int _curPropCount = 0;

    private Dictionary<StageItem, int> _itemDict = new();
    private Dictionary<PropController, int> _carDict = new();
    private Dictionary<PropController, int> _planeDict = new();

    private void Start()
    {        
        EventBus.Instance.Subscribe<EventPropRemoved>(OnPropRemoved);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<EventPropRemoved>(OnPropRemoved);
    }

    #region Player
    //최초 플레이어 프리팹 로드 및 생성
    //TODO : 씬 외부에서 선택된 플레이어 정보를 넘겨주고 해당 데이터를 받아 생성한다.
    public PlayerController SpawnPlayer(PlayerInfo info)
    {
        var player = AddressableManager.Instance.Spawn(info.PrefapName, null);
        var controller = player.GetComponent<PlayerController>();
        controller.SetData(info);
        controller.Block();
        RespawnPlayer(controller, true);
        return controller;
    }
    public void RespawnPlayer(PlayerController player, bool isStart = false)
    {
        player.transform.position = _playerSpawnPos.position;
        player.transform.rotation = _playerSpawnPos.rotation;

        StartCoroutine(RespawnDelay(player, isStart));
    }
    private IEnumerator RespawnDelay(PlayerController controller, bool isStart)
    {
        int timer = Constants.PLAYER_SPAWN_TIME;

        var sec = new WaitForSeconds(1f);
        while (timer >= 0)
        {
            EventBus.Instance.Publish(new EventSpawnTimer(timer));
            timer--;
            yield return sec;
        }

        controller.Respawn();
        if (isStart)
        {
            EventBus.Instance.Publish(new EventStartGame());
        }
    }

    #endregion Player

    #region Props

    public void SpawnProps()
    {
        StartCoroutine(SpawnBasicPropsCoroutine());
        StartCoroutine(SpawnCarPropsCoroutine());
        StartCoroutine(SpawnPlanePropsCoroutine());
    }

    private IEnumerator SpawnBasicPropsCoroutine()
    {
        //TODO : 일시 정지 상태가 있으면 변경필요
        var sec = new WaitForSeconds(1f);
        while(StageManager.Instance.IsSpawnable)
        {
            yield return sec;
            if (_curPropCount < _maxPropCount)
            {
                SelectProp("Basic");
            }
        }
    }

    private IEnumerator SpawnCarPropsCoroutine()
    {
        var sec = new WaitForSeconds(10f);
        while (StageManager.Instance.IsSpawnable)
        {
            yield return sec;
            if (_curPropCount < _maxPropCount)
            {
                int count = _carGenPosList.Count;
                List<int> posList = new();
                for (int i = 0; i < count; i++)
                {
                    //빈자리 찾아서 랜덤
                    if (!_carDict.ContainsValue(i))
                    {
                        posList.Add(i);
                    }
                }

                if (posList.Count > 0)
                {
                    int rnd = Random.Range(0, posList.Count);
                    var car = SelectProp("Car", _carGenPosList[posList[rnd]]);
                    if (car != null)
                    {
                        _carDict.Add(car, posList[rnd]);
                    }
                }
            }
        }
    }
    private IEnumerator SpawnPlanePropsCoroutine()
    {
        var sec = new WaitForSeconds(10f);
        while (StageManager.Instance.IsSpawnable)
        {
            yield return sec;
            if (_curPropCount < _maxPropCount)
            {
                int count = _planeGenPosList.Count;
                List<int> posList = new();
                for (int i = 0; i < count; i++)
                {
                    //빈자리 찾아서 랜덤
                    if (!_planeDict.ContainsValue(i))
                    {
                        posList.Add(i);
                    }
                }

                if (posList.Count > 0)
                {
                    int rnd = Random.Range(0, posList.Count);
                    var plane = SelectProp("Plane", _planeGenPosList[posList[rnd]]);
                    if (plane != null)
                    {
                        _planeDict.Add(plane, posList[rnd]);
                    }
                }
            }
        }
    }
    private PropController SelectProp(string type, Transform parent = null)
    {
        var rateTable = TableManager.Instance.GetProbByType(type);

        int curProb = 0;
        int rnd = Random.Range(0, 10000);
        int groupId = 0;
        foreach (var rate in rateTable)
        {
            curProb += rate.Prob;
            if (rnd < curProb)
            {
                groupId = rate.GroupId;
                break;
            }
        }

        var targets = TableManager.Instance.GetPropGroupList(groupId);
        curProb = 0;
        rnd = Random.Range(0, 10000);
        foreach (var prop in targets)
        {
            curProb += prop.GenRate;
            if (rnd < curProb)
            {
                return GenerateProp(prop, parent);
            }
        }

        return null;
    }

    private PropController GenerateProp(JPropInfoData data, Transform parent = null)
    {
        var prop = AddressableManager.Instance.Spawn(data.PrefabName, parent);
        var controller = prop.GetComponent<PropController>();
        if(parent != null)
        {
            controller.SetData(data);
        }
        else
        {
            int rnd = Random.Range(0, _propGenerateArea.Count);
            var bound = _propGenerateArea[rnd].bounds;
            controller.SetData(data, bound);
        }

        _curPropCount++;
        return controller;
    }

    private void OnPropRemoved(EventPropRemoved e)
    {
        if (_carDict.ContainsKey(e.Prop))
        {
            _carDict.Remove(e.Prop);
        }
        else if (_planeDict.ContainsKey(e.Prop))
        {
            _planeDict.Remove(e.Prop);
        }

        AddressableManager.Instance.Release(e.Prop.gameObject);
        _curPropCount--;
    }

    #endregion Props

    #region Item
    public void SpawnItems()
    {
        StartCoroutine(SpawnItemCoroutine());
    }

    private IEnumerator SpawnItemCoroutine()
    {
        var sec = new WaitForSeconds(10f);
        while (StageManager.Instance.IsSpawnable)
        {
            yield return sec;
            int count = _itemSpawnPosList.Count;
            for (int i = 0; i < count; i++)
            {
                if (!_itemDict.ContainsValue(i))
                {
                    _itemDict.Add(SelectItem(i), i);
                    break;
                }
            }
        }
    }

    private StageItem SelectItem(int pos)
    {
        var items = TableManager.Instance.GetStageItemList();
        int rnd = Random.Range(0, items.Count);
        var table = items[rnd];

        var itemPrefab = AddressableManager.Instance.Spawn("StageItem", _itemSpawnPosList[pos]);
        itemPrefab.transform.localPosition = Vector3.zero;
        var item = itemPrefab.GetComponent<StageItem>();
        item.SetData(table);

        return item;
    }

    public void RemoveItem(StageItem item)
    {
        if(_itemDict.ContainsKey(item))
        {
            _itemDict.Remove(item);
            AddressableManager.Instance.Release(item.gameObject);
        }
    }
    #endregion Item
}
