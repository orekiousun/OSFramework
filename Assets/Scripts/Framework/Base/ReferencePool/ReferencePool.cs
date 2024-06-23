using System;
using System.Collections.Generic;

namespace OSFramework
{
    public static partial class ReferencePool
    {
        private static readonly Dictionary<Type, ReferenceCollection> s_ReferenceCollections = new Dictionary<Type, ReferenceCollection>();
        private static bool m_EnableStrictCheck = false;
        
        /// <summary>
        /// 获取设置是否开启强制检查（检查类型是否合规）
        /// </summary>
        public static bool EnableStrictCheck
        {
            get
            {
                return m_EnableStrictCheck;
            }
            set
            {
                m_EnableStrictCheck = value;
            }
        }

        /// <summary>
        /// 获取引用池的数量
        /// </summary>
        public static int Count
        {
            get
            {
                return s_ReferenceCollections.Count;
            }
        }

        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            int index = 0;
            ReferencePoolInfo[] results = null;

            lock (s_ReferenceCollections)
            {
                results = new ReferencePoolInfo[s_ReferenceCollections.Count];
                foreach (KeyValuePair<Type, ReferenceCollection> referenceCollection in s_ReferenceCollections)
                {
                    results[index++] = new ReferencePoolInfo(referenceCollection.Key, referenceCollection.Value.UnusedReferencesCount, 
                        referenceCollection.Value.UsingReferenceCount, referenceCollection.Value.AcquireReferenceCount, referenceCollection.Value.ReleaseReferenceCount, 
                        referenceCollection.Value.AddReferenceCount, referenceCollection.Value.RemoveReferenceCount);
                }
            }

            return results;
        }
        
        /// <summary>
        /// 清除所有引用池
        /// </summary>
        public static void ClearAll()
        {
            lock (s_ReferenceCollections)
            {
                foreach (KeyValuePair<Type, ReferenceCollection> referenceCollection in s_ReferenceCollections)
                {
                    referenceCollection.Value.RemoveAll();
                }
                
                s_ReferenceCollections.Clear();
            }
        }
        
        /// <summary>
        /// 从引用池获取引用 
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>引用</returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }
        
        /// <summary>
        /// 引用池获取引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <returns></returns>
        public static IReference Acquire(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Acquire();
        }
        
        /// <summary>
        /// 释放引用（当前引用不再使用，归还到引用池中暂存起来，以便下次获得）
        /// </summary>
        /// <param name="reference">引用</param>
        public static void Release(IReference reference)
        {
            if (reference == null)
            {
                throw new OSFrameworkException("Reference is invalid.");
            }

            Type referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference);
        }
        
        /// <summary>
        /// 向引用池中添加指定数量的引用
        /// </summary>
        /// <param name="count">指定数量</param>
        /// <typeparam name="T">引用类型</typeparam>
        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }
        
        /// <summary>
        /// 向引用池中添加指定数量的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">指定数量</param>
        public static void Add(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Add(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="count">指定数量</param>
        /// <typeparam name="T">引用类型</typeparam>
        public static void Remove<T>(int count) where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }
        
        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">指定数量</param>
        public static void Remove(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Remove(count);
        }

        /// <summary>
        /// 从引用池中移除所有引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }
        
        /// <summary>
        /// 从引用池中移除所有引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        public static void RemoveAll(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }
        

        /// <summary>
        /// 判断类似是否合规，where T : class, IReference, new()
        /// </summary>
        /// <param name="referenceType"></param>
        /// <exception cref="OSFrameworkException"></exception>
        private static void InternalCheckReferenceType(Type referenceType)
        {
            if (!m_EnableStrictCheck)
            {
                return;
            }

            if (referenceType == null) 
            {
                throw new OSFrameworkException("Reference type is invalid.");
            }

            if (!referenceType.IsClass || referenceType.IsAbstract)
            {
                throw new OSFrameworkException("Reference type is not a non-abstract class type.");
            }

            if (!typeof(IReference).IsAssignableFrom(referenceType))
            {
                throw new OSFrameworkException(Utility.Text.Format("Reference type '{0}' is invalid.", referenceType.FullName));
            }
        }

        /// <summary>
        /// 获取引用Collection，如果没有就实例化一个新的
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <returns></returns>
        private static ReferenceCollection GetReferenceCollection(Type referenceType)
        {
            if (referenceType == null)
            {
                throw new OSFrameworkException("ReferenceType is invalid");
            }

            ReferenceCollection referenceCollection = null;
            lock (s_ReferenceCollections)
            {
                if (!s_ReferenceCollections.TryGetValue(referenceType, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(referenceType);
                    s_ReferenceCollections.Add(referenceType, referenceCollection);
                }
            }

            return referenceCollection;
        }
    }   
}
