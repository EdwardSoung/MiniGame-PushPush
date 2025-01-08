using FirstVillain.Converter;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class MenuCommands
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
