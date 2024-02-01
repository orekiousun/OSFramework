using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSFramework
{
    public static class OSFrameworkEntry
    {
        private static readonly OSFrameworkLinkedList<OSFrameworkModule> s_OSFrameworkModules = new OSFrameworkLinkedList<OSFrameworkModule>();

        /// <summary>
        /// 模块轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (OSFrameworkModule module in s_OSFrameworkModules)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }
        
        /// <summary>
        /// 关闭并清理所有模块
        /// </summary>
        public static void Shutdown()
        {
            // 按照优先级顺序从后往前一个一个关闭
            for (LinkedListNode<OSFrameworkModule> current = s_OSFrameworkModules.Last;
                current != null;
                current = current.Previous)
            {
                current.Value.Shutdown();   
            }
            
            s_OSFrameworkModules.Clear();
            
        }
    }
}
