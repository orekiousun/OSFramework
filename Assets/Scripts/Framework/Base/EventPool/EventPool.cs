using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace OSFramework
{
    /// <summary>
    /// 事件池
    /// </summary>
    internal sealed partial class EventPool<T> where T : BaseEventArgs
    {
        private readonly OSFrameworkMultiDictionary<int, EventHandler<T>> m_EventHandlers;
        private readonly Queue<Event> m_Events;
        private readonly Dictionary<object, LinkedListNode<EventHandler<T>>> m_CachedNodes;
        private readonly Dictionary<object, LinkedListNode<EventHandler<T>>> m_TempNodes;
        private readonly EventPoolMode m_EventPoolMode;
        private EventHandler<T> m_DefaultHandler;

        /// <summary>
        /// 实例化事件池
        /// </summary>
        /// <param name="mode">事件池模式</param>
        public EventPool(EventPoolMode mode)
        {
            m_EventHandlers = new OSFrameworkMultiDictionary<int, EventHandler<T>>();
            m_Events = new Queue<Event>();
            m_CachedNodes = new Dictionary<object, LinkedListNode<EventHandler<T>>>();
            m_TempNodes = new Dictionary<object, LinkedListNode<EventHandler<T>>>();
            m_EventPoolMode = mode;
            m_DefaultHandler = null;
        }

        /// <summary>
        /// 获取事件处理函数的数量
        /// </summary>
        public int EventHandlerCount
        {
            get
            {
                return m_EventHandlers.Count;
            }
        }

        /// <summary>
        /// 获取事件数量
        /// </summary>
        public int EventCount
        {
            get
            {
                return m_Events.Count;
            }
        }

        /// <summary>
        /// 事件池轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            lock (m_Events)
            {
                while (m_Events.Count > 0)
                {
                    Event eventNode = m_Events.Dequeue();
                    HandleEvent(eventNode.Sender, eventNode.EventArgs);
                    ReferencePool.Release(eventNode);
                }
            }
        }

        /// <summary>
        /// 处理事件结点（即执行事件函数）
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        private void HandleEvent(object sender, T e)
        {
            bool noHandlerException = false;
            OSFrameworkLinkedListRange<EventHandler<T>> range = default(OSFrameworkLinkedListRange<EventHandler<T>>);
            if (m_EventHandlers.TryGetValue(e.Id, out range))
            {
                // 通过Id获取到m_EventHandlers中的一系列注册进去的函数
                LinkedListNode<EventHandler<T>> current = range.First;
                while (current != null && current != range.Terminal)
                {
                    m_CachedNodes[e] = current.Next != range.Terminal ? current.Next : null;
                    current.Value(sender, e);  // 执行事件函数
                    current = m_CachedNodes[e];
                }

                m_CachedNodes.Remove(e);
            }
            else if(m_DefaultHandler != null)
            {
                // 如果从m_EventHandlers没有获取到对应Id的事件，则执行默认事件
                m_DefaultHandler(sender, e);
            }
            else if((m_EventPoolMode & EventPoolMode.AllowNoHandler) == 0)
            {
                // 表示当前的模式需要有事件来支持，但是没有获取到事件，要抛出异常
                noHandlerException = true;
            }

            ReferencePool.Release(e);

            if (noHandlerException)
            {
                throw new OSFrameworkException(Utility.Text.Format("Event '{0}' not allow no handler.", e.Id));
            }
        }
        
        /// <summary>
        /// 关闭并清理事件池
        /// </summary>
        public void shutDown()
        {
            Clear();
            m_EventHandlers.Clear();
            m_CachedNodes.Clear();
            m_TempNodes.Clear();
            m_DefaultHandler = null;
        }
        
        /// <summary>
        /// 清理事件
        /// </summary>
        public void Clear()
        {
            lock (m_Events)
            {
                m_Events.Clear();
            }
        }
        
        /// <summary>
        /// 获取事件处理函数的数量
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <returns>事件处理函数的数量</returns>
        public int Count(int id)
        {
            OSFrameworkLinkedListRange<EventHandler<T>> range = default(OSFrameworkLinkedListRange<EventHandler<T>>);
            if (m_EventHandlers.TryGetValue(id, out range))
            {
                return range.Count;
            }

            return 0;
        }
        
        /// <summary>
        /// 检查是否存在事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">要检查的事件处理函数</param>
        /// <returns>是否存在事件处理函数</returns>
        public bool Check(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new OSFrameworkException("Event handler is invalid.");
            }

            return m_EventHandlers.Contains(id, handler);
        }
        
        /// <summary>
        /// 订阅事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">要订阅的事件处理函数</param>
        public void Subscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new OSFrameworkException("Event handler is invalid.");
            }

            if (!m_EventHandlers.Contains(id))
            {
                m_EventHandlers.Add(id, handler);
            }
            else if ((m_EventPoolMode & EventPoolMode.AllowMultiHandler) != EventPoolMode.AllowMultiHandler)
            {
                // m_EventPoolMode不为AllowMultiHandler
                throw new OSFrameworkException(Utility.Text.Format("Event '{0}' not allow multi handler.", id));
            }
            else if ((m_EventPoolMode & EventPoolMode.AllowDuplicateHandler) != EventPoolMode.AllowDuplicateHandler && Check(id, handler))
            {
                // m_EventPoolMode不为AllowDuplicateHandler，且不存在该事件处理函数
                throw new OSFrameworkException(Utility.Text.Format("Event '{0}' not allow duplicate handler.", id));
            }
            else
            {
                m_EventHandlers.Add(id, handler);
            }
        }

        /// <summary>
        /// 取消订阅事件处理函数
        /// </summary>
        /// <param name="id">事件类型编号</param>
        /// <param name="handler">要取消订阅的事件处理函数</param>
        public void Unsubscribe(int id, EventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new OSFrameworkException("Event handler is invalid.");
            }

            if (m_CachedNodes.Count > 0)
            {
                foreach (KeyValuePair<object, LinkedListNode<EventHandler<T>>> cachedNode in m_CachedNodes)
                {
                    if (cachedNode.Value != null && cachedNode.Value.Value == handler)
                    {
                        m_TempNodes.Add(cachedNode.Key, cachedNode.Value.Next);
                    }
                }

                if (m_TempNodes.Count > 0)
                {
                    foreach (KeyValuePair<object, LinkedListNode<EventHandler<T>>> tempNode in m_TempNodes)
                    {
                        m_CachedNodes[tempNode.Key] = tempNode.Value;
                    }
                    
                    m_TempNodes.Clear();
                }
            }

            if (!m_EventHandlers.Remove(id, handler))
            {
                throw new OSFrameworkException(Utility.Text.Format("Event '{0}' not exists special handler.", id));
            }
        }

        /// <summary>
        /// 设置默认事件处理函数
        /// </summary>
        /// <param name="handler"></param>
        public void SetDefaultHandler(EventHandler<T> handler)
        {
            m_DefaultHandler = handler;
        }

        /// <summary>
        /// 抛出事件（这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发，在Upadate中执行后事件从m_Events中移除）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="OSFrameworkException"></exception>
        public void Fire(object sender, T e)
        {
            if (e == null)
            {
                throw new OSFrameworkException("Event is invalid.");
            }
            
            Event eventNode = Event.Create(sender, e);
            lock (m_Events)
            {
                m_Events.Enqueue(eventNode);
            }
        }
        
        /// <summary>
        /// 立即抛出事件，这个操作不是线程安全的，事件会立即分发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        public void FireNow(object sender, T e)
        {
            if (e == null)
            {
                throw new OSFrameworkException("Event is invalid.");
            }
            
            HandleEvent(sender, e);
        }
    }
}