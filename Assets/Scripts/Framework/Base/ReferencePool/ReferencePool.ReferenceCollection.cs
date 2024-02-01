using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSFramework
{
    public static partial class ReferencePool
    {
        /// <summary>
        /// 引用统计类，根据构造的引用类型统计有多少同类的引用，并进行了哪些操作
        /// </summary>
        private sealed class ReferenceCollection
        {
            /// <summary>
            /// 没有使用的引用队列
            /// </summary>
            private readonly Queue<IReference> m_References;
            
            private readonly Type m_ReferenceType;
            private int m_UsingReferenceCount;
            private int m_AcquireReferenceCount;
            private int m_ReleaseReferenceCount;
            private int m_AddReferenceCount;
            private int m_RemoveReferenceCount;
            
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="referenceType">引用类型</param>
            public ReferenceCollection(Type referenceType)
            {
                m_References = new Queue<IReference>();
                m_ReferenceType = referenceType;
                m_UsingReferenceCount = 0;
                m_AcquireReferenceCount = 0;
                m_ReleaseReferenceCount = 0;
                m_AddReferenceCount = 0;
                m_RemoveReferenceCount = 0;
            }
            
            /// <summary>
            /// 引用类型
            /// </summary>
            public Type ReferenceType
            {
                get
                {
                    return m_ReferenceType;
                }
            }
            
            /// <summary>
            /// 未使用的引用个数
            /// </summary>
            public int UnusedReferencesCount
            {
                get
                {
                    return m_References.Count;
                }
            }
            
            /// <summary>
            /// 在使用的引用个数
            /// </summary>
            public int UsingReferenceCount
            {
                get
                {
                    return m_UsingReferenceCount;
                }
            }
            
            /// <summary>
            /// 获取的引用个数
            /// </summary>
            public int AcquireReferenceCount
            {
                get
                {
                    return m_AcquireReferenceCount;
                }
            }

            /// <summary>
            /// 释放的引用个数
            /// </summary>
            public int ReleaseReferenceCount
            {
                get
                {
                    return m_ReleaseReferenceCount;
                }
            }

            /// <summary>
            /// 添加的引用个数
            /// </summary>
            public int AddReferenceCount
            {
                get
                {
                    return m_AddReferenceCount;
                }
            }

            /// <summary>
            /// 移除的引用个数
            /// </summary>
            public int RemoveReferenceCount
            {
                get
                {
                    return m_RemoveReferenceCount;
                }
            }
            
            /// <summary>
            /// 获取到某个引用类型，返回对应类型的实例
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns></returns>
            public T Acquire<T>() where T : class, IReference, new()
            {
                if (typeof(T) != m_ReferenceType)
                {
                    throw new OSFrameworkException("Type is invalid.");
                }
                
                // 使用次数和获取次数都++
                m_UsingReferenceCount++;
                m_AcquireReferenceCount++;
                lock (m_References)  // 防止被其他线程修改
                {
                    if (m_References.Count > 0)
                    {
                        return (T) m_References.Dequeue();
                    }
                }
                
                // 如果没有对象，则新建一个，创建次数++
                m_AddReferenceCount++;
                return new T();
            }
            
            /// <summary>
            /// 获取到某个引用类型，返回IReference
            /// </summary>
            /// <returns></returns>
            public IReference Acquire()
            {
                m_UsingReferenceCount++;
                m_AcquireReferenceCount++;
                lock (m_References)
                {
                    if (m_References.Count > 0)
                    {
                        return m_References.Dequeue();
                    }
                }

                m_AddReferenceCount++;
                return (IReference)Activator.CreateInstance(m_ReferenceType);
            }

            /// <summary>
            /// 释放引用（归还引用池）
            /// </summary>
            /// <param name="reference">引用</param>
            public void Release(IReference reference)
            {
                reference.Clear();
                lock (m_References)
                {
                    if (m_EnableStrictCheck && m_References.Contains(reference))
                    {
                        throw new OSFrameworkException("The Exception has been released");
                    }
                    
                    // 释放引用后，引用回归到没有使用的引用队列
                    m_References.Enqueue(reference);
                }

                m_ReleaseReferenceCount++;
                m_UsingReferenceCount--;
            }
            
            /// <summary>
            /// 添加引用
            /// </summary>
            /// <param name="count">添加到引用个数</param>
            /// <typeparam name="T"></typeparam>
            /// <exception cref="OSFrameworkException"></exception>
            public void Add<T>(int count) where T : class, IReference, new()
            {
                if (typeof(T) != m_ReferenceType)
                {
                    throw new OSFrameworkException("Type is invalid.");
                }

                lock (m_References)
                {
                    m_AddReferenceCount += count;
                    while (count-- > 0)
                    {
                        m_References.Enqueue(new T());
                    }
                }
            }

            /// <summary>
            /// 添加引用
            /// </summary>
            /// <param name="count">添加引用的个数</param>
            public void Add(int count)
            {
                lock (m_References)
                {
                    m_AddReferenceCount += count;

                    while (count-- > 0)
                    {
                        m_References.Enqueue((IReference)Activator.CreateInstance(m_ReferenceType));
                    }
                }
            }

            /// <summary>
            /// 移除引用
            /// </summary>
            /// <param name="count">移除引用的个数</param>
            public void Remove(int count)
            {
                lock (m_References)
                {
                    if (count > m_References.Count)
                    {
                        count = m_References.Count;
                    }

                    m_RemoveReferenceCount += count;
                    while (count-- > 0)
                    {
                        m_References.Dequeue();
                    }
                }
            }
            
            /// <summary>
            /// 移除所有引用
            /// </summary>
            public void RemoveAll()
            {
                lock (m_References)
                {
                    m_RemoveReferenceCount += m_References.Count;
                    m_References.Clear();
                }
            }
        }
    }
}
