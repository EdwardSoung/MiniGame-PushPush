using FirstVillain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XSystem.Converter;
using XSystem.Singleton;

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
}
