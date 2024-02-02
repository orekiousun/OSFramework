using System;

namespace OSFramework
{
    /// <summary>
    /// 包含事件数据的类的基类
    /// </summary>
    public abstract class OSFrameworkEventArgs : EventArgs, IReference
    {
        /// <summary>
        /// 实例化包含事件数据的类的基类
        /// </summary>
        public OSFrameworkEventArgs()
        {
        }
        
        /// <summary>
        /// 清理引用
        /// </summary>
        public abstract void Clear();
    }
}