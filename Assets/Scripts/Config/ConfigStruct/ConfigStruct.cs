using System.Collections.Generic;
using System;

namespace ConfigStruct
{
    [Serializable]
    public class BaseType
    {
        public List<BaseTypeItem> type;
    }
    
    [Serializable]
    public class BaseTypeItem
    {
        public string type;
        public string name;
    }
}
