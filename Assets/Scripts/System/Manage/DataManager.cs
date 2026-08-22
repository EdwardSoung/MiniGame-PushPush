using XSystem.Singleton;
using UnityEngine;

public class DataManager : UnitySingleton<DataManager>
{
    public PropModule Prop {  get; private set; }
    public StageItemModule StageItem {  get; private set; }
    public StageRewardModule StageReward {  get; private set; }
    public RewardGroupModule RewardGroup {  get; private set; }
    public ItemModule Item {  get; private set; }

    public void Init()
    {
        Prop = new PropModule();
        StageItem = new StageItemModule();
        StageReward = new StageRewardModule();
        RewardGroup = new RewardGroupModule();
        Item = new ItemModule();
    }
}
