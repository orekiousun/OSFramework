using System;

namespace OSFramework
{
    /// <summary>
    /// 变量
    /// </summary>
    public abstract class Variable : IReference
    {
        /// <summary>
        /// 实例化变量（虽然抽象类不能被直接实例化，但是可以有默认的构造函数） 
        /// </summary>
        public Variable()
        {
        }

        /// <summary>
        /// 获取变量类型
        /// </summary>
        public abstract Type Type
        {
            get;
        }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <returns>变量值</returns>
        public abstract object GetValue();

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="value">变量值</param>
        public abstract void SetValue(object value);
        
        /// <summary>
        /// 清理变量值
        /// </summary>
        public abstract void Clear();
    }   
}