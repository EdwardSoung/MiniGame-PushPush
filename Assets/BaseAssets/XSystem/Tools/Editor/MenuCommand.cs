using System.IO;
using UnityEditor;
using UnityEngine;
using XSystem.Converter;


public static class MenuCommand
{
    [MenuItem("Tools/Convert/ExcelToJson", priority = 999)]
    public static void ConvertExcelToJson()
    {
        var tablePath = Application.dataPath.Replace("Assets", "Table");
        var jsonPath = Path.Combine(Application.dataPath, "AddressableResources", "Tables");
        var entityPath = Path.Combine(Application.dataPath, "Scripts", "Entities");
        var enumPath = Path.Combine(Application.dataPath, "Scripts", "Etc");

        JsonConverter.ExcelToJsonAndClass(tablePath, jsonPath, entityPath, enumPath);

        AssetDatabase.Refresh();
    }
}
