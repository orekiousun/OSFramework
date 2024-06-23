using System;
using System.Collections.Generic;

namespace OSFramework
{
    /// <summary>
    /// 游戏入口（感觉也可以叫ModuleManager，主要处理模块的轮询，关闭和获取以及创建）
    /// </summary>
    public static class OSFrameworkEntry
    {
        // 从前往后，优先级递减
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
            ReferencePool.ClearAll();
            // TODO:
            Utility.Marshal.FreeCachedHGlobal();
            OSFrameworkLog.SetLogHelper(null);
        }
        
        /// <summary>
        /// 获取模块，会检查模块是否合规
        /// </summary>
        /// <typeparam name="T">模块类型</typeparam>
        /// <returns>如果获取的模块不存在，就自动创建该模块</returns>
        public static T GetModule<T>() where T : class
        {
            Type interfaceType = typeof(T);
            if (!interfaceType.IsInterface)
            {
                throw new OSFrameworkException(Utility.Text.Format("You must get Module by interface, but '{0}' is not.", interfaceType.FullName));
            }

            if (!interfaceType.FullName.StartsWith("OSFramework", StringComparison.Ordinal))
            {
                throw new OSFrameworkException(Utility.Text.Format("You must get a OS Framework module, but '{0}' is not.", interfaceType.FullName));
            }

            string moduleName = Utility.Text.Format("{0}.{1}", interfaceType.Namespace, interfaceType.Name.Substring(1));
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                throw new OSFrameworkException(Utility.Text.Format("Can not find OS Framework module type '{0}'", moduleName));
            }

            return GetModule(moduleType) as T;
        }
        
        /// <summary>
        /// 获取模块
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>如果获取的模块不存在，就自动创建该模块</returns>
        private static OSFrameworkModule GetModule(Type moduleType)
        {
            foreach (OSFrameworkModule module in s_OSFrameworkModules)
            {
                if (module.GetType() == moduleType)
                {
                    return module;
                }
            }

            return CreateModule(moduleType);
        }
        
        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="moduleType">模块类型</param>
        /// <returns>返回创建的模块</returns>
        private static OSFrameworkModule CreateModule(Type moduleType)
        {
            OSFrameworkModule module = (OSFrameworkModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new OSFrameworkException(Utility.Text.Format("Can not create module '{0}'", moduleType.FullName));
            }

            LinkedListNode<OSFrameworkModule> current = s_OSFrameworkModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                s_OSFrameworkModules.AddBefore(current, module); // 添加在前面，保证优先级顺序
            }
            else
            {
                s_OSFrameworkModules.AddLast(module);            // 表示当前需要添加到模块优先级最小，需要添加在最后
            }

            return module;
        }
    }
}
