namespace OSFramework
{
    /// <summary>
    /// 任务基类
    /// </summary>
    public abstract class TaskBase : IReference
    {
        /// <summary>
        /// 默认优先级
        /// </summary>
        public const int DefaultPriority = 0;

        private int m_SerialId;
        private string m_Tag;
        private int m_Proirity;
        private object m_UserData;

        private bool m_Done;

        /// <summary>
        /// 实例化任务基类
        /// </summary>
        public TaskBase()
        {
            m_SerialId = 0;
            m_Tag = null;
            m_Proirity = DefaultPriority;
            m_Done = false;
            m_UserData = null;
        }

        /// <summary>
        /// 获取任务序列编号
        /// </summary>
        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        /// <summary>
        /// 获取任务标签
        /// </summary>
        public string Tag
        {
            get
            {
                return m_Tag;
            }
        }

        /// <summary>
        /// 获取任务优先级
        /// </summary>
        public int Proirity
        {
            get
            {
                return m_Proirity;
            }
        }

        /// <summary>
        /// 获取任务用户自定义数据
        /// </summary>
        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }
        
        /// <summary>
        /// 获取或设置任务是否完成
        /// </summary>
        public bool Done
        {
            get
            {
                return m_Done;
            }
            set
            {
                m_Done = value;
            }
        }

        /// <summary>
        /// 获取任务描述
        /// </summary>
        public virtual string Description
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 初始化任务基类
        /// </summary>
        /// <param name="serialId">任务序列编号</param>
        /// <param name="tag">任务标签</param>
        /// <param name="priority">任务优先级</param>
        /// <param name="userData">任务的用户自定义数据</param>
        internal void Initialize(int serialId, string tag, int priority, object userData)
        {
            m_SerialId = serialId;
            m_Tag = tag;
            m_Proirity = priority;
            m_UserData = userData;
            m_Done = false;
        }
        
        /// <summary>
        /// 清理任务基类
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Clear()
        {
            m_SerialId = 0;
            m_Tag = null;
            m_Proirity = DefaultPriority;
            m_UserData = null;
            m_Done = false;
        }
    }
}