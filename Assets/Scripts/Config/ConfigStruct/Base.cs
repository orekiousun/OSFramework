using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ConfigStruct;

[Serializable]
public class Base
{
    public List<BaseTypeItem> type;
    public List<BaseItem> data;
}

[Serializable]
public class BaseItem
{
	public int id;
	public string name;
	public int value;
}
