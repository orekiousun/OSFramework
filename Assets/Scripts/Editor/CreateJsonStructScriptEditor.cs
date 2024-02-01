using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ConfigStruct;
using OSUtils;

public class CreateJsonStructScriptEditor
{
    private const string JsonScriptTemplateName = "JsonAssetScriptTemplate.cs.txt";      // 模板文件名
    private const string DataTemplate = "\tpublic #DataType# #ValueName#;\n";            // 行模板
    
    /// <summary>
    /// 根据json表自动创建配置类脚本
    /// </summary>
    [MenuItem("Assets/Create/Script/JsonStructScript", false)]
    static void CreateStructScript()
    {
        string savePath = EditorUtility.SaveFolderPanel("Save ExcelAssetScript", Application.dataPath + "/Scripts/Config/ConfigStruct", "");
        if(savePath == "") return;

        var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);  // 选中的文件

        string jsonPath = AssetDatabase.GetAssetPath(selectedAssets[0]);                                       // 选中的json文件路径
        string jsonName = Path.GetFileNameWithoutExtension(jsonPath);                                          // json文件名
        string path = Path.ChangeExtension(Path.Combine(savePath, jsonName), "cs");                   // 保存的cs文件名
        
        string scriptString = BuildJsonScriptString(jsonName, jsonPath);
        File.WriteAllText(path, scriptString);
        
        Debug.Log(String.Format("{0}类创建成功", jsonName));
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 检测是否可以创建结构体脚本
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/Create/Script/JsonStructScript", true)]
    static bool CreateJsonScriptValidation()
    {
        var selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
        if(selectedAssets.Length != 1) return false;
        var path = AssetDatabase.GetAssetPath(selectedAssets[0]);
        return Path.GetExtension(path) == ".json";
    }
    
    /// <summary>
    /// 创建对应结构体脚本
    /// </summary>
    /// <param name="jsonName">json文件名字（与结构体名字一一对应）</param>
    /// <param name="jsonPath">json文件位置</param>
    /// <returns></returns>
    static string BuildJsonScriptString(string jsonName, string jsonPath)
    {
        string scriptString = ConfigUtils.GetScriptTemplateString(JsonScriptTemplateName);

        scriptString = scriptString.Replace("#Name#", jsonName);
        string str = "";
        string jsonStr = File.ReadAllText(jsonPath);
        BaseType types = JsonUtility.FromJson<BaseType>(jsonStr);
        foreach (var item in types.type)
        {
            str += DataTemplate;
            str = str.Replace("#DataType#", item.type);
            str = str.Replace("#ValueName#", item.name);
        }
        str = str.Remove(str.Length - 1, 1);  // 移除结尾的/n(只占一个字符)
        scriptString = scriptString.Replace("#ItemFields#", str);
        return scriptString;
    }
}


