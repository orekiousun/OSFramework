namespace OSFramework
{
    /// <summary>
    /// 模块抽象类
    /// </summary>
    internal abstract class OSFrameworkModule
    {
        /// <summary>
        /// 模块优先级
        /// </summary>
        /// <remarks>优先级高的模块会优先更新，并且关闭操作会滞后进行</remarks>>
        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 模块初始化
        /// </summary>
        internal abstract void Init(); 
        
        /// <summary>
        /// 模块更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理模块
        /// </summary>
        internal abstract void Shutdown();
    }
}

