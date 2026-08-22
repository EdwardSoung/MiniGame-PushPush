using FirstVillain.Entities;
using System.Collections.Generic;

public class StageItemModule
{

    private List<JStageItemData> stageItemList = new();
    private Dictionary<int, JStageItemData> stageItemById = new();

    public int StageItemCount
    { 
        get {  return stageItemList.Count; }  
    }

    private List<JStageItemEffectData> stageItemEffectList= new();

    private Dictionary<int, JStageItemEffectData> stageItemEffectById = new();
    public StageItemModule()
    {
        stageItemList = TableManager.Instance.LoadTable<JStageItemData>(E_TABLE.JStageItem).list;
        foreach (var item in stageItemList)
        {
            stageItemById.Add(item.Id, item);
        }

        stageItemEffectList = TableManager.Instance.LoadTable<JStageItemEffectData>(E_TABLE.JStageItemEffect).list;

        foreach (var itemEffect in stageItemEffectList)
        {
            stageItemEffectById.Add(itemEffect.Id, itemEffect);
        }
    }

    public JStageItemData GetStageItemById(int id)
    {
        return stageItemById.GetValueOrDefault(id);
    }

    public JStageItemEffectData GetStageItemEffectById(int id)
    {
        return stageItemEffectById.GetValueOrDefault(id);
    }
}
