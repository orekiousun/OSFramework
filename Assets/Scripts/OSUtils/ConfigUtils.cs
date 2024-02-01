using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace OSUtils
{
    public class ConfigUtils
    {
        /// <summary>
        /// 获取保存的模板
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetScriptTemplateString(string templateName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string[] filePath = Directory.GetFiles(currentDirectory, templateName, SearchOption.AllDirectories);
            if(filePath.Length == 0) throw new Exception("Script template not found.");

            string templateString = File.ReadAllText(filePath[0]);
            return templateString;
        }
        
        private const string JsonSuffix = ".json"; 
        private static string _jsonFile = Application.dataPath + "/Resources/GameConfig";
        /// <summary>
        /// 获取json配置文件路径
        /// </summary>
        /// <param name="jsonName"></param>
        /// <returns></returns>
        public static string GetJsonPath(string jsonName)
        {
            string path = Path.Combine(_jsonFile, jsonName);
            path = Path.ChangeExtension(path, JsonSuffix);
            return path;
        }
    }
}
