using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ConfigStruct;
using OSUtils;

public class BaseConfig
{
	private static string _jsonPath;
	
	private static List<BaseItem> _data;
    public static List<BaseItem> Data
    {
        get
        {
            if (_data == null)
                LoadConfig();
            return _data;
        }
    }

	private static void LoadConfig()
	{
	    _jsonPath = ConfigUtils.GetJsonPath("Base");
		string jsonStr = File.ReadAllText(_jsonPath);
		Base config = JsonUtility.FromJson<Base>(jsonStr);
		_data = config.data;
	}
	
	public static void Release()
    {
        _data = null;
    }
    
    
}
