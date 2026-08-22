using FirstVillain.Entities;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RewardGroupModule
{
    private List<JRewardGroupData> rewardGroupList = new();

    private Dictionary<int, List<JRewardGroupData>> rewardGroupByGroupId = new();
    public RewardGroupModule()
    {
        rewardGroupList = TableManager.Instance.LoadTable<JRewardGroupData>(E_TABLE.JRewardGroup).list;

        foreach (var groupItem in rewardGroupList)
        {
            if(!rewardGroupByGroupId.TryGetValue(groupItem.GroupId, out var list))
            {
                rewardGroupByGroupId[groupItem.GroupId] = new List<JRewardGroupData>();
            }

            rewardGroupByGroupId[groupItem.GroupId].Add(groupItem);
        }
    }

    public List<JRewardGroupData> GetRewardsByGroupId(int groupId)
    {
        return rewardGroupByGroupId.GetValueOrDefault(groupId);
    }
}
