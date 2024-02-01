using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace OSFramework
{
    /// <summary>
    /// 异常类
    /// </summary>
    [Serializable]
    public class OSFrameworkException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public OSFrameworkException()
            : base()
        {
        }
        
        /// <summary>
        /// 使用指定消息实例化异常类
        /// </summary>
        /// <param name="message"></param>
        public OSFrameworkException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化实例
        /// </summary>
        /// <param name="message">解释异常原因的错误消息</param>
        /// <param name="innerException">导致当前异常的异常。如果 innerException 参数不为空引用，则在处理内部异常的 catch 块中引发当前异常</param>
        public OSFrameworkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        /// <summary>
        /// 用序列化数据初始化实例
        /// </summary>
        /// <param name="info">引发异常的序列化数据</param>
        /// <param name="context">包含有关源或目标的上下文信息</param>
        protected OSFrameworkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        
    }
}

