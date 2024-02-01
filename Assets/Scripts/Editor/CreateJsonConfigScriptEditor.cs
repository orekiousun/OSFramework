using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ProjectWindowCallback;
using System.IO;
using UnityEditor;
using System;
using OSUtils;

public class CreateJsonConfigScriptEditor
{
    const string ConfigScriptTemplateName = "ConfigAssetScriptTemplate.cs.txt";  // 模板文件名
    
    /// <summary>
    /// 根据模板生成脚本
    /// </summary>
    [MenuItem("Assets/Create/Script/JsonConfigScript", false)]
    static void CreateConfigScript()
    {
        var icon = EditorGUIUtility.FindTexture("cs Script Icon");
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
            ScriptableObject.CreateInstance<CreateScriptAssetAction>(),
            GetSelectedPathOrFallback() + "/xxxConfig.cs",
            icon,
            ConfigScriptTemplateName);
    }
    
    /// <summary>
    /// 获取当前选中文件路径
    /// </summary>
    /// <returns></returns>
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    /// <summary>
    /// 创建对应配置脚本
    /// </summary>
    /// <param name="jsonName">json文件名字（与结构体名字一一对应）</param>
    /// <param name="jsonPath">json文件位置</param>
    /// <returns></returns>
    public static string BuildConfigScriptString(string jsonName, string template)
    {
        string scriptString = ConfigUtils.GetScriptTemplateString(template);
        scriptString = scriptString.Replace("#Name#", jsonName);
        return scriptString;
    }

}

class CreateScriptAssetAction : EndNameEditAction
{
    public override void Action(int instanceId, string pathName, string resourceFile)
    {
        UnityEngine.Object obj = CreateAssetFromTemplate(pathName, resourceFile);  // 创建资源
        ProjectWindowUtil.ShowCreatedAsset(obj);                                           // 高亮显示该资源
    }
    
    internal static UnityEngine.Object CreateAssetFromTemplate(string pathName, string template)
    {
        // 获取脚本
        string configName = Path.GetFileNameWithoutExtension(pathName);
        string configClass = configName.Replace("Config", "");
        string scriptString = CreateJsonConfigScriptEditor.BuildConfigScriptString(configClass, template);  // 修改脚本内容

        // 保存脚本
        File.WriteAllText(pathName, scriptString);
        AssetDatabase.ImportAsset(pathName);
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
    }
}
