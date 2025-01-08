using FirstVillain.EventBus;

public class EventStartStage : EventBase
{
    public PlayerInfo SelectedPlayer { get; private set; }
    public E_STAGE_TYPE StageType { get; private set; }

    public EventStartStage(PlayerInfo info, E_STAGE_TYPE stageType)
    {
        SelectedPlayer= info;
        StageType = stageType;
    }
}

public class EventMinigameStop : EventBase
{
    
}

public class EventSendMinigamePoint : EventBase
{
    public float MinigamePoint { get; private set; }

    public EventSendMinigamePoint(float point)
    {
        MinigamePoint = point;
    }
}

public class EventPropRemoved : EventBase
{ 
    public PropController Prop { get; private set; }

    public EventPropRemoved(PropController controller)
    {
        Prop = controller;
    }
}

public class EventSpawnTimer : EventBase
{
    public int Timer { get; private set; }

    public EventSpawnTimer(int timer)
    {
        Timer = timer;
    }
}

public class EventStartGame : EventBase
{

}

public class EventPlayTimer : EventBase
{
    public int Timer { get; private set; }
    
    public EventPlayTimer(int timer)
    {
        Timer = timer;
    }
}

public class EventUpdateScore : EventBase
{
    public E_TEAM Team { get; private set; }
    public int Score { get; private set; }

    public EventUpdateScore(E_TEAM team, int score)
    {
        Team = team;
        Score = score;
    }
}

public class EventUseItem : EventBase
{
    public int EffectId { get; private set; }

    public EventUseItem(int id)
    {
        EffectId = id;
    }
}

public class EventItemRemoved : EventBase
{
    public StageItem Item { get; private set; }

    public EventItemRemoved(StageItem item)
    {
        Item = item;
    }
}