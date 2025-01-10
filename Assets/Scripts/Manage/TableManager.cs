using FirstVillain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        var asset = AddressableManager.Instance.LoadAssetAsync<TextAsset>(table.ToString());
        return JsonConvert.DeserializeObject<Wrapper<T>>(asset.text);
    }

    #region LoadAssets


    #endregion LoadAssets
    //���̺��� ���̺� �������� �Լ��� ��� �߰��� ����ϴ� ����...
    //���� �޸𸮿� �÷��ΰ� ����ϴ°� ������ ��� �ʿ�
    #region Prop
    public JPropInfoData GetPropInfoById(int id)
    {
        var list = LoadTableAsset<JPropInfoData>(E_TABLE.JPropInfo).list;
        return list.Find(arg => arg.Id == id);
    }

    public List<JPropInfoData> GetPropInfoList(E_TABLE table)
    {
        return LoadTableAsset<JPropInfoData>(table).list;
    }

    public List<JPropInfoData> GetPropGroupList(int groupId)
    {
        var list = LoadTableAsset<JPropInfoData>(E_TABLE.JPropInfo).list;
        return list.FindAll(arg => arg.PropGroupId == groupId);
    }

    public List<JPropRateData> GetProbByType(string type)
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
}
