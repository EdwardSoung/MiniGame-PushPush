#region Stage
public enum E_TEAM
{
    None,
    Red,
    Blue,
}

public enum E_STAGE_TYPE
{
    Single_Adventure,
    Single_TimeAttack,
}

public enum E_STAGE_STATE
{
    Ready,
    Playing,
    Pause,
    End,
}
#endregion Stage

#region UI
public enum E_UI_ROOT_TYPE
{
    UI,
    Loading,
    System,
}

public enum E_UI_TYPE
{
    UIPanelIntro  = 1,
    UIPanelLoadingIntro,
    UIPanelLoading,

    UIPanelStage = 100,
    UIPanelStageResult,

    UIPanelLobby = 200,
    UIPanelPlayerList,
}
#endregion UI

#region Table
public enum E_TABLE
{
    JPropInfo,
    JPlayer,
    JPropRate,
    JStageItem,
    JStageItemEffect,
    JStageReward,
    JItem,
}
#endregion Table