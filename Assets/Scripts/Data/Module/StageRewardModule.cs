using FirstVillain.Entities;
using System.Collections.Generic;

public class StageRewardModule
{
    private List<JStageRewardData> stageRewardList = new();

    public StageRewardModule()
    {
        stageRewardList = TableManager.Instance.LoadTable<JStageRewardData>(E_TABLE.JStageReward).list;
    }

    public JStageRewardData GetStageReward(int point)
    {
        JStageRewardData curReward = null;

        foreach (var reward in stageRewardList)
        {
            curReward = reward;

            if (point < reward.Point)
            {
                break;
            }
        }

        return curReward;
    }
}
