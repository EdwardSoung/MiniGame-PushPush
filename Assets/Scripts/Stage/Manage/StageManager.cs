using FirstVillain.Entities;
using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUnityLibrary.Singleton;

public class StageManager : UnitySingletonOnce<StageManager>
{
    [SerializeField] private SpawnController _spawnController;

    private PlayerController _currentPlayer;
    private Dictionary<E_TEAM, int> _teamScoreDict = new();
    private Dictionary<int, int> _gotPropDict = new();

    private int _coinItem = 0;
    private int _gemItem = 0;

    private E_STAGE_TYPE _currentStageType;
    private E_STAGE_STATE _state;

    public bool IsSpawnable { get { return _state <= E_STAGE_STATE.Playing; } }

    #region Initialize
    public void Init(PlayerInfo player, E_STAGE_TYPE type)
    {
        _currentStageType = type;
        _coinItem = 0;
        _gemItem = 0;
        _state = E_STAGE_STATE.Ready;
        InitScore();
        //TODO : UI초기화
        _currentPlayer = _spawnController.SpawnPlayer(player);       
        _spawnController.SpawnProps();
        _spawnController.SpawnItems();
    }

    private void InitScore()
    {
        switch(_currentStageType)
        {
            case E_STAGE_TYPE.Single_Adventure:
            case E_STAGE_TYPE.Single_TimeAttack:
                _teamScoreDict.Add(E_TEAM.Red, 0);
                break;
        }
    }

    #endregion Initialize

    #region Play Control
    public void StartGame()
    {
        _state = E_STAGE_STATE.Playing;
        StartCoroutine(PlayTimer());        
    }
    public void GameOver()
    {
        _state = E_STAGE_STATE.End;
        _currentPlayer.Block();
        var ui = UIManager.Instance.OpenUI(E_UI_TYPE.UIPanelStageResult) as UIPanelStageResult;
        ui.SetData(_gotPropDict);
    }

    public void GameEnd()
    {
        _currentPlayer.GameOver();
    }
    #endregion Play Control

    public void UpdateScore(E_TEAM team, JPropInfoData prop)
    {
        if (_teamScoreDict.ContainsKey(team))
        {
            //획득 목록도 팀별로...?
            if(_gotPropDict.ContainsKey(prop.Id))
            {
                _gotPropDict[prop.Id]++;
            }
            else
            {
                _gotPropDict.Add(prop.Id, 1);
            }
            _teamScoreDict[team] += prop.Point;
            EventBus.Instance.Publish(new EventUpdateScore(team, _teamScoreDict[team]));            
        }
    }

    public void RemoveItem(StageItem item)
    {
        _spawnController.RemoveItem(item);
    }

    public void UseItem(JStageItemEffectData data)
    {
        switch(data.EffectType)
        {
            default:
                //효과 적용
                break;
            case "Coin":
                _coinItem += data.Value;
                break;
            case "Gem":
                _gemItem += data.Value;
                break;
        }
    }

    //플레이어 낙하했을 때
    public void PlayerFall(PlayerController player)
    {
        _spawnController.RespawnPlayer(player);
    }

    #region Timer
    private IEnumerator PlayTimer()
    {
        float timer = Constants.PLAY_TIME;
        int sec = (int)Constants.PLAY_TIME;
        float secTimer = 0;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            secTimer += Time.deltaTime;
            if(secTimer >= 1f)
            {
                secTimer = 0;
                sec--;
                EventBus.Instance.Publish(new EventPlayTimer(sec));
            }
            yield return null;
        }

        GameOver();
    }
    #endregion Timer
}
