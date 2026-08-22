using FirstVillain.Entities;
using System.Collections.Generic;
using UnityEngine;

public class PropModule 
{
    private List<JPropInfoData> propInfoList = new();
    private Dictionary<int, JPropInfoData> PropInfoById = new();
    private Dictionary<int, List<JPropInfoData>> PropInfoByGroupId = new();

    private List<JPropRateData> propRateList;

    private Dictionary<string, List<JPropRateData>> PropRateByType = new();

    public PropModule()
    {
        propInfoList = TableManager.Instance.LoadTable<JPropInfoData>(E_TABLE.JPropInfo).list;

        foreach (var propInfo in propInfoList)
        {
            PropInfoById.Add(propInfo.Id, propInfo);

            if (!PropInfoByGroupId.TryGetValue(propInfo.PropGroupId, out var list))
            {
                PropInfoByGroupId[propInfo.PropGroupId] = new List<JPropInfoData>();
            }

            PropInfoByGroupId[propInfo.PropGroupId].Add(propInfo);
        }

        propRateList = TableManager.Instance.LoadTable<JPropRateData>(E_TABLE.JPropRate).list;
        foreach (var propRate in propRateList)
        {
            if(!PropRateByType.TryGetValue(propRate.PropType, out var list))
            {
                PropRateByType[propRate.PropType] = new List<JPropRateData>();
            }

            PropRateByType[propRate.PropType].Add(propRate);
        }
    }
    public JPropInfoData GetPropInfoById(int id)
    {
        return PropInfoById.GetValueOrDefault(id);
    }

    public List<JPropInfoData> GetPropGroupList(int groupId)
    {
        return PropInfoByGroupId.GetValueOrDefault(groupId);
    }

    public List<JPropRateData> GetProbRateByType(string type)
    {
        return PropRateByType.GetValueOrDefault(type);
    }
}
