using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstVillain.Entities;

public class SingleStageSceneController : MonoBehaviour
{
    private UIPanelStage _stageUI;
    private void Awake()
    {
        EventBus.Instance.Subscribe<EventStartStage>(OnStartStage);
        EventBus.Instance.Subscribe<EventSpawnTimer>(OnUpdateSapwnTimer);
        EventBus.Instance.Subscribe<EventStartGame>(OnStartGame);
        EventBus.Instance.Subscribe<EventPlayTimer>(OnUpdatePlayTimer);
        EventBus.Instance.Subscribe<EventUpdateScore>(OnUpdateScore);
        EventBus.Instance.Subscribe<EventUseItem>(OnUseItem);
        EventBus.Instance.Subscribe<EventItemRemoved>(OnItemRemoved);
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<EventStartStage>(OnStartStage);
        EventBus.Instance.Unsubscribe<EventSpawnTimer>(OnUpdateSapwnTimer);
        EventBus.Instance.Unsubscribe<EventStartGame>(OnStartGame);
        EventBus.Instance.Unsubscribe<EventPlayTimer>(OnUpdatePlayTimer);
        EventBus.Instance.Unsubscribe<EventUpdateScore>(OnUpdateScore);
        EventBus.Instance.Unsubscribe<EventUseItem>(OnUseItem);
        EventBus.Instance.Unsubscribe<EventItemRemoved>(OnItemRemoved);
    }

    private void OnStartStage(EventStartStage e)
    {
        //UI »ý¼º
        _stageUI = UIManager.Instance.OpenUI(E_UI_TYPE.UIPanelStage) as UIPanelStage;
        StageManager.Instance.Init(e.SelectedPlayer, e.StageType);
    }

    private void OnUpdateSapwnTimer(EventSpawnTimer e)
    {
        _stageUI.UpdateSpawnTimer(e.Timer);
    }

    private void OnStartGame(EventStartGame e)
    {
        StageManager.Instance.StartGame();
    }

    private void OnUpdatePlayTimer(EventPlayTimer e)
    {
        _stageUI.UpdatePlayTimer(e.Timer);
    }

    private void OnUpdateScore(EventUpdateScore e)
    {
        _stageUI.UpdateScore(e.Team, e.Score);
    }

    private void OnUseItem(EventUseItem e)
    {
        var effect = TableManager.Instance.GetStageItemEffect(e.EffectId);
        StageManager.Instance.UseItem(effect);
    }

    private void OnItemRemoved(EventItemRemoved e)
    {
        StageManager.Instance.RemoveItem(e.Item);
    }
}
