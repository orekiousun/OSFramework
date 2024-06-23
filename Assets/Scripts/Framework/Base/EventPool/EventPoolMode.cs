using System;

namespace OSFramework
{
    /// <summary>
    /// 事件池模式
    /// </summary>
    [Flags] // 使用[Flags]标签即可以像整数一样进行按位的“|”或者按位的“&”操作了。
    internal enum EventPoolMode : byte
    {
        /// <summary>
        /// 默认事件池模式，即必须存在有且只有一个事件处理函数
        /// </summary>
        Default = 0,
        
        /// <summary>
        /// 允许不存在事件处理函数
        /// </summary>
        AllowNoHandler = 1,
        
        /// <summary>
        /// 允许存在多个事件处理函数
        /// </summary>
        AllowMultiHandler = 2,
        
        /// <summary>
        /// 允许存在重复的事件处理函数
        /// </summary>
        AllowDuplicateHandler = 4
    }
}