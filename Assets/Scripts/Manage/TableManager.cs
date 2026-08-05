using FirstVillain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XUnityLibrary.Converter;
using XUnityLibrary.Singleton;

public class TableManager : UnitySingleton<TableManager>
{
    public Wrapper<T> LoadTable<T>(E_TABLE table)
    {
        return LoadTableAsset<T>(table);
    }

    private Wrapper<T> LoadTableAsset<T>(E_TABLE table)
    {
        var asset = AddressableManager.Instance.LoadAsset<TextAsset>(table.ToString());
        return JsonConvert.DeserializeObject<Wrapper<T>>(asset.text);
    }

    #region LoadAssets


    #endregion LoadAssets
    //테이블별로 테이블 가져오는 함수를 계속 추가해 줘야하는 단점...
    //전부 메모리에 올려두고 사용하는게 나을지 고민 필요
    #region Prop
    public JPropInfoData GetPropInfoById(int id)
    {
        var list = LoadTableAsset<JPropInfoData>(E_TABLE.JPropInfo).list;
        return list.Find(arg => arg.Id == id);
    }

    public List<JPropInfoData> GetPropInfoList()
    {
        return LoadTableAsset<JPropInfoData>(E_TABLE.JPropInfo).list;
    }

    public List<JPropInfoData> GetPropGroupList(int groupId)
    {
        var list = LoadTableAsset<JPropInfoData>(E_TABLE.JPropInfo).list;
        return list.FindAll(arg => arg.PropGroupId == groupId);
    }

    public List<JPropRateData> GetProbRateByType(string type)
    {
        var list = LoadTableAsset<JPropRateData>(E_TABLE.JPropRate).list;
        return list.FindAll(arg => arg.PropType == type);
    }

    #endregion Prop

    #region Stage Item
    public List<JStageItemData> GetStageItemList()
    {
        return LoadTableAsset<JStageItemData>(E_TABLE.JStageItem).list;
    }
    public JStageItemEffectData GetStageItemEffect(int id)
    {
        var list = LoadTableAsset<JStageItemEffectData>(E_TABLE.JStageItemEffect).list;
        return list.Find(arg => arg.Id == id);
    }
    #endregion Stage Item

    #region StageReward

    public JStageRewardData GetStageReward(int point)
    {
        var list = LoadTableAsset<JStageRewardData>(E_TABLE.JStageReward).list;

        JStageRewardData curReward = null;

        foreach (var reward in list)
        {
            curReward = reward;

            if(point < reward.Point)
            {
                break;
            }
        }

        return curReward;
    }

    #endregion StageReward

    #region Reward Group

    public List<JRewardGroupData> GetRewardsByGroupId(int groupId)
    {
        var list = LoadTableAsset<JRewardGroupData>(E_TABLE.JRewardGroup).list;

        return list.Where(x => x.GroupId == groupId).ToList();
    }

    #endregion Reward Group

    #region Item

    public JItemData GetItem(int id)
    {
        var list = LoadTableAsset<JItemData>(E_TABLE.JItem).list;

        return list.Find(x => x.Id == id);
    }

    #endregion Item
}
