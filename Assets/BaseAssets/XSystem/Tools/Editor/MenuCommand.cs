using System.IO;
using UnityEditor;
using UnityEngine;
using XUnityLibrary.Converter;


public static class MenuCommand
{
    [MenuItem("Tools/Convert/ExcelToJson", priority = 999)]
    public static void ConvertExcelToJson()
    {
        var tablePath = Application.dataPath.Replace("Assets", "TableData");
        var jsonPath = Path.Combine(Application.dataPath, "AddressableResources", "Tables");
        var entityPath = Path.Combine(Application.dataPath, "Scripts", "Entities");

        JsonConverter.ExcelToJsonAndClass(tablePath, jsonPath, entityPath);

        AssetDatabase.Refresh();
    }
}
