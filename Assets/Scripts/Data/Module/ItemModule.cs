using UnityEngine;
using System.Collections.Generic;
using FirstVillain.Entities;
public class ItemModule
{
    private List<JItemData> itemList = new();
    private Dictionary<int, JItemData> itemByDataId = new();

    public ItemModule()
    {
        itemList = TableManager.Instance.LoadTable<JItemData>(E_TABLE.JItem).list;

        foreach (var item in itemList)
        {
            itemByDataId.Add(item.Id, item);
        }
    }

    public JItemData GetItem(int id)
    {
        return itemByDataId.GetValueOrDefault(id);
    }
}
