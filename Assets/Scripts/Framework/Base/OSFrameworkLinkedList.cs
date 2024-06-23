using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OSFramework
{
    /// <summary>
    /// 链表类
    /// </summary>
    /// <typeparam name="T">链表元素类型</typeparam>
    public sealed class OSFrameworkLinkedList<T> : ICollection<T>, ICollection, IEnumerable<T>, IEnumerable
    {
        private readonly LinkedList<T> m_LinkedList;
        /// <summary>
        /// 存放被释放了还没有使用的节点
        /// </summary>
        private readonly Queue<LinkedListNode<T>> m_CachedNodes;

        /// <summary>
        /// 构造函数
        /// </summary>
        public OSFrameworkLinkedList()
        {
            m_LinkedList = new LinkedList<T>();
            m_CachedNodes = new Queue<LinkedListNode<T>>();
        }

        /// <summary>
        /// 链表中结点数量
        /// </summary>
        public int Count
        {
            get
            {
                return m_LinkedList.Count;
            }
        }

        /// <summary>
        /// 节点缓存数量
        /// </summary>
        public int CachedNodeCount
        {
            get
            {
                return m_CachedNodes.Count;
            }
        }

        /// <summary>
        /// 获取链表第一个节点
        /// </summary>
        public LinkedListNode<T> First
        {
            get
            {
                return m_LinkedList.First;
            }
        }
        
        /// <summary>
        /// 获取链表最后一个节点
        /// </summary>
        public LinkedListNode<T> Last
        {
            get
            {
                return m_LinkedList.Last;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示 ICollection 是否为只读
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<T>) m_LinkedList).IsReadOnly;
            }
        }

        /// <summary>
        /// 获取可用于同步对 ICollection 的访问的对象
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return ((ICollection) m_LinkedList).SyncRoot;
            }
        }
        
        /// <summary>
        ///  获取一个值，该值指示是否同步对 ICollection 的访问（线程安全）
        /// </summary>
        public bool IsSynchronized 
        {
            get
            {
                return ((ICollection) m_LinkedList).IsSynchronized;
            }
        }

        /// <summary>
        /// 在链表中某个现有节点后面添加一个指定值的新节点
        /// </summary>
        /// <param name="node">现有节点</param>
        /// <param name="value">新节点的值</param>
        /// <returns>包含指定值的节点</returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = AcquireNode(value);
            m_LinkedList.AddAfter(node, newNode);
            return newNode;
        }
        
        /// <summary>
        /// 在链表中某个现有节点后面添加一个新节点
        /// </summary>
        /// <param name="node">现有节点</param>
        /// <param name="newNode">新节点</param>
        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_LinkedList.AddAfter(node, newNode);
        }
        
        /// <summary>
        /// 在链表中某个现有节点前面添加一个指定值的新节点
        /// </summary>
        /// <param name="node">现有节点</param>
        /// <param name="value">新节点的值</param>
        /// <returns>包含指定值的节点</returns>
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = AcquireNode(value);
            m_LinkedList.AddBefore(node, newNode);
            return newNode;
        }
        
        /// <summary>
        /// 在链表中某个现有节点前面添加一个新节点
        /// </summary>
        /// <param name="node">现有节点</param>
        /// <param name="newNode">新节点</param>
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_LinkedList.AddBefore(node, newNode);
        }
        
        /// <summary>
        /// 在开头添加一个指定值的新节点
        /// </summary>
        /// <param name="value">新节点的值</param>
        /// <returns>包含指定值的节点</returns>
        public LinkedListNode<T> AddFirst(T value)
        {
            LinkedListNode<T> node = AcquireNode(value);
            m_LinkedList.AddFirst(node);
            return node;
        }
        
        /// <summary>
        /// 在开头添加一个新节点
        /// </summary>
        /// <param name="node">新节点</param>
        public void AddFirst(LinkedListNode<T> node)
        {
            m_LinkedList.AddFirst(node);
        }
        
        /// <summary>
        /// 在结尾添加一个指定值的新节点
        /// </summary>
        /// <param name="value">新节点的值</param>
        /// <returns>包含指定值的节点</returns>
        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> node = AcquireNode(value);
            m_LinkedList.AddLast(node);
            return node;
        }
        
        /// <summary>
        /// 在结尾添加一个新节点
        /// </summary>
        /// <param name="node">新节点</param>
        public void AddLast(LinkedListNode<T> node)
        {
            m_LinkedList.AddLast(node);
        }
        
        /// <summary>
        /// 获取一个节点（如果节点缓存中还有节点就从缓存中取，如如果没有就新创建一个）
        /// </summary>
        /// <param name="value">节点值</param>
        /// <returns></returns>
        private LinkedListNode<T> AcquireNode(T value)
        {
            LinkedListNode<T> node = null;
            if (m_CachedNodes.Count > 0)
            {
                node = m_CachedNodes.Dequeue();
                node.Value = value;
            }
            else
            {
                node = new LinkedListNode<T>(value);
            }

            return node;
        }

        /// <summary>
        /// 移除链表中所有节点
        /// </summary>
        public void Clear()
        {
            LinkedListNode<T> current = m_LinkedList.First;
            while (current != null)
            {
                ReleaseNode(current);
                current = current.Next;
            }
            
            m_LinkedList.Clear();
        }
        
        /// <summary>
        /// 释放节点（将释放了的节点放入m_CachedNodes节点缓存中）
        /// </summary>
        /// <param name="node">要释放的节点</param>
        private void ReleaseNode(LinkedListNode<T> node)
        {
            node.Value = default(T);
            m_CachedNodes.Enqueue(node);
        }
        
        /// <summary>
        /// 清空节点缓存
        /// </summary>
        public void ClearCachedNodes()
        {
            m_CachedNodes.Clear();
        }

        /// <summary>
        /// 确定链表中是否有某值
        /// </summary>
        /// <param name="value">指定值</param>
        /// <returns>是否有某值</returns>
        public bool Contains(T value)
        {
            return m_LinkedList.Contains(value);
        }
        
        /// <summary>
        /// 从指定索引处开始将链表复制到一个数组中
        /// </summary>
        /// <param name="array">一维数组，它是从链表复制的元素的目标。数组必须具有从零开始的索引</param>
        /// <param name="index">指定索引</param>
        public void CopyTo(T[] array, int index)
        {
            m_LinkedList.CopyTo(array, index);
        }
        
        /// <summary>
        /// 从指定索引处开始将链表复制到一个数组中
        /// </summary>
        /// <param name="array">一维数组，它是从 ICollection 复制的元素的目标。数组必须具有从零开始的索引</param>
        /// <param name="index">指定索引</param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)m_LinkedList).CopyTo(array, index);
        }
        
        /// <summary>
        /// 从链表中寻找包含指定值的第一个节点
        /// </summary>
        /// <param name="value">要查找的指定值</param>
        /// <returns>包含指定值的第一个节点</returns>
        public LinkedListNode<T> Find(T value)
        {
            return m_LinkedList.Find(value);
        }
        
        /// <summary>
        /// 从链表中寻找包含指定值的最后一个节点
        /// </summary>
        /// <param name="value">要查找的指定值</param>
        /// <returns>包含指定值的最后一个节点</returns>
        public LinkedListNode<T> FindLast(T value)
        {
            return m_LinkedList.FindLast(value);
        }
        
        /// <summary>
        /// 移除链表中指定值的第一个匹配项
        /// </summary>
        /// <param name="value">指定的值</param>
        /// <returns>是否移除成功</returns>
        public bool Remove(T value)
        {
            LinkedListNode<T> node = m_LinkedList.Find(value);
            if (node != null)
            {
                m_LinkedList.Remove(node);
                ReleaseNode(node);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 从链表中移除指定节点
        /// </summary>
        /// <param name="node">指定节点</param>
        public void Remove(LinkedListNode<T> node)
        {
            m_LinkedList.Remove(node);
            ReleaseNode(node);
        }

        /// <summary>
        /// 移除链表开头的节点
        /// </summary>
        public void RemoveFirst()
        {
            LinkedListNode<T> first = m_LinkedList.First;
            if (first == null)
            {
                throw new OSFrameworkException("First is invalid.");
            }
            
            m_LinkedList.RemoveFirst();
            ReleaseNode(first);
        }

        /// <summary>
        /// 移除链表结尾的节点
        /// </summary>
        public void RemoveLast()
        {
            LinkedListNode<T> last = m_LinkedList.Last;
            if (last == null)
            {
                throw new OSFrameworkException("Last is invalid.");
            }
            
            m_LinkedList.RemoveLast();
            ReleaseNode(last);
        }

        /// <summary>
        /// 返回循环访问集合的枚举数（有点类似C++的迭代器）
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_LinkedList);
        }
        
        /// <summary>
        /// 返回循环访问集合的枚举数（有点类似C++的迭代器）
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <summary>
        /// 返回循环访问集合的枚举数（有点类似C++的迭代器）
        /// </summary>
        /// <returns>循环访问集合的枚举数</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 将值添加到ICollection的结尾处
        /// </summary>
        /// <param name="value">要添加的值</param>
        void ICollection<T>.Add(T value)
        {
            AddLast(value);
        }

        /// <summary>
        /// 循环访问集合的枚举数（实现了类似C++的迭代器）
        /// </summary>
        [StructLayout(LayoutKind.Auto)]  // 定义结构体在内存中的布局方式
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private LinkedList<T>.Enumerator m_Enumerator;
            
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="linkedList">传入的链表</param>
            internal Enumerator(LinkedList<T> linkedList)
            {
                if (linkedList == null)
                {
                    throw new OSFrameworkException("Linked list is invalid.");
                }

                m_Enumerator = linkedList.GetEnumerator();
            }
            
            /// <summary>
            /// 获取当前节点
            /// </summary>
            public T Current
            {
                get
                {
                    return m_Enumerator.Current;
                }
            }
            
            /// <summary>
            /// 获取当前的枚举数
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return m_Enumerator.Current;
                }
            }

            /// <summary>
            /// 清理枚举数
            /// </summary>
            public void Dispose()
            {
                m_Enumerator.Dispose();
            }
            
            /// <summary>
            /// 获取下一个节点
            /// </summary>
            /// <returns>返回下一个节点</returns>
            public bool MoveNext()
            {
                return m_Enumerator.MoveNext();
            }
            
            /// <summary>
            /// 重置枚举数
            /// </summary>
            void IEnumerator.Reset()
            {
                ((IEnumerator<T>)m_Enumerator).Reset();
            }
        }
    }
}

