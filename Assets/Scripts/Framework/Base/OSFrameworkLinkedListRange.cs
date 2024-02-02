using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OSFramework
{
    /// <summary>
    /// 链表范围（获取到链表的一个范围）
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public class OSFrameworkLinkedListRange<T> : IEnumerable<T>, IEnumerable
    {
        private readonly LinkedListNode<T> m_First;
        private readonly LinkedListNode<T> m_Terminal;

        /// <summary>
        /// 实例化链表范围
        /// </summary>
        /// <param name="first">链表范围开始节点</param>
        /// <param name="terminal">链表范围结束节点</param>
        /// <exception cref="OSFrameworkException"></exception>
        public OSFrameworkLinkedListRange(LinkedListNode<T> first, LinkedListNode<T> terminal)
        {
            if (first == null || terminal == null || first == terminal)
            {
                throw new OSFrameworkException("Range is valid.");
            }

            m_First = first;
            m_Terminal = terminal;
        }

        /// <summary>
        /// 获取链表范围是否有效
        /// </summary>
        public bool IsValid
        {
            get
            {
                return m_First != null && m_Terminal != null && m_First != m_Terminal;
            }
        }

        /// <summary>
        /// 获取链表范围开始节点
        /// </summary>
        public LinkedListNode<T> First
        {
            get
            {
                return m_First;
            }
        }

        /// <summary>
        /// 获取链表范围结束节点
        /// </summary>
        public LinkedListNode<T> Terminal
        {
            get
            {
                return m_Terminal;
            }
        }
        
        /// <summary>
        /// 获取链表节点数量
        /// </summary>
        public int Count
        {
            get
            {
                if (!IsValid)
                {
                    return 0;
                }

                int count = 0;
                for (LinkedListNode<T> current = m_First; current != null && current != m_Terminal; current = current.Next)
                {
                    count++;
                }

                return count;
            }
        }
        
        /// <summary>
        /// 检查链表范围是否包含指定值
        /// </summary>
        /// <param name="value">指定值</param>
        /// <returns>是否包含指定值</returns>
        public bool Contains(T value)
        {
            for (LinkedListNode<T> current = m_First; current != null && current != m_Terminal; current = current.Next)
            {
                if (current.Value.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 返回循环访问的枚举数
        /// </summary>
        /// <returns>循环访问的枚举数</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// 返回循环访问的枚举数
        /// </summary>
        /// <returns>循环访问的枚举数</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 返回循环访问的枚举数
        /// </summary>
        /// <returns>循环访问的枚举数</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 循环访问集合的枚举数
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly OSFrameworkLinkedListRange<T> m_OSFrameworkLinkedListRange;
            private LinkedListNode<T> m_Current;
            private T m_CurrentValue;
            
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="range">链表范围</param>
            internal Enumerator(OSFrameworkLinkedListRange<T> range)
            {
                if (!range.IsValid)
                {
                    throw new OSFrameworkException("Range is invalid.");
                }

                m_OSFrameworkLinkedListRange = range;
                m_Current = m_OSFrameworkLinkedListRange.m_First;
                m_CurrentValue = default(T);
            }
            
            /// <summary>
            /// 获取当前节点
            /// </summary>
            public T Current
            {
                get
                {
                    return m_CurrentValue;
                }
            }
            
            /// <summary>
            /// 获取当前枚举数
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return m_CurrentValue;
                }
            }
            
            /// <summary>
            /// 清理枚举数
            /// </summary>
            /// <exception cref="NotImplementedException"></exception>
            public void Dispose()
            {
            }
            
            /// <summary>
            /// 获取下一个节点
            /// </summary>
            /// <returns>返回下一个节点</returns>
            public bool MoveNext()
            {
                if (m_Current == null || m_Current == m_OSFrameworkLinkedListRange.m_Terminal)
                {
                    return false;
                }

                m_CurrentValue = m_Current.Value;
                m_Current = m_Current.Next;
                return true;
            }
            
            /// <summary>
            /// 重置枚举数
            /// </summary>
            public void Reset()
            {
                m_Current = m_OSFrameworkLinkedListRange.m_First;
                m_CurrentValue = default(T);
            }
        }
    }
}

