using System.Collections.Generic;

namespace OSFramework
{
    /// <summary>
    /// 任务池
    /// </summary>
    /// <typeparam name="T"><任务类型/typeparam>
    internal sealed class TaskPool<T> where T : TaskBase
    {
        /// <summary>
        /// 空闲的任务代理
        /// </summary>
        private readonly Stack<ITaskAgent<T>> m_FreeAgents;
        /// <summary>
        /// 正在运行的任务（须有任务代理持有任务）
        /// </summary>
        private readonly OSFrameworkLinkedList<ITaskAgent<T>> m_WorkingAgents;
        /// <summary>
        /// 等待中的任务（不需要任务持有任务，但是要运行起来的话需要由任务代理来持有） 
        /// </summary>
        private readonly OSFrameworkLinkedList<T> m_WaitingTasks;
        private bool m_Paused;

        /// <summary>
        /// 实例化任务池
        /// </summary>
        public TaskPool()
        {
            m_FreeAgents = new Stack<ITaskAgent<T>>();
            m_WorkingAgents = new OSFrameworkLinkedList<ITaskAgent<T>>();
            m_WaitingTasks = new OSFrameworkLinkedList<T>();
            m_Paused = false;
        }

        /// <summary>
        /// 获取任务池是否被暂停
        /// </summary>
        public bool Paused
        {
            get
            {
                return m_Paused;
            }
            set
            {
                m_Paused = value;
            }
        }

        /// <summary>
        /// 获取任务代理总数量
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return FreeAgentsCount + WorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取可用任务代理数量
        /// </summary>
        public int FreeAgentsCount
        {
            get
            {
                return m_FreeAgents.Count;
            }
        }

        /// <summary>
        /// 获取工作中任务代理数量
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return m_WorkingAgents.Count;
            }
        }
        
        /// <summary>
        /// 获取等待任务数量
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_WaitingTasks.Count;
            }
        }

        /// <summary>
        /// 任务池轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_Paused)
            {
                return;
            }
            
            ProcessRunningTask(elapseSeconds, realElapseSeconds);
            ProcessWaitingTasks(elapseSeconds, realElapseSeconds);
        }
        
        /// <summary>
        /// 处理正在运行的任务
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        private void ProcessRunningTask(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<ITaskAgent<T>> current = m_WorkingAgents.First;
            while (current != null)
            {
                T task = current.Value.Task;
                if (!task.Done)
                {
                    // 如果任务还未完成，就继续执行任务
                    current.Value.Update(elapseSeconds, realElapseSeconds);
                    current = current.Next;
                    continue;
                }
                
                LinkedListNode<ITaskAgent<T>> next = current.Next;
                // 任务已经完成，结束任务代理的工作
                current.Value.Reset();
                m_FreeAgents.Push(current.Value);
                m_WorkingAgents.Remove(current);
                ReferencePool.Release(task);
                current = next;
            }
        }
        
        /// <summary>
        /// 处理正在等待的任务
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        private void ProcessWaitingTasks(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<T> current = m_WaitingTasks.First;
            while (current != null && FreeAgentsCount > 0)
            {
                // 先给任务一个任务代理
                ITaskAgent<T> agent = m_FreeAgents.Pop();
                LinkedListNode<ITaskAgent<T>> agentNode = m_WorkingAgents.AddLast(agent);
                T task = current.Value;
                LinkedListNode<T> next = current.Next;
                StartTaskStatus status = agent.Start(task);  // 由任务代理来开启任务，返回一个接下来的任务状态，以确定任务的下一步操作
                // status == StartTaskStatus.Done || status == StartTaskStatus.UnknownError：均视为任务完成
                // status == StartTaskStatus.HasToWait：不能继续处理此任务，需要等到其他任务完成--重置任务，此时任务代理为空闲，将任务保留到等待队列中
                // status == StartTaskStatus.CanResume：可以继续处理此任务--将任务从等待队列中移除（m_WorkingAgents继续持有当前任务代理）
                if (status == StartTaskStatus.Done || status == StartTaskStatus.HasToWait || status == StartTaskStatus.UnknownError)
                {
                    agent.Reset();
                    m_FreeAgents.Push(agent);
                    m_WorkingAgents.Remove(agentNode);
                }

                if (status == StartTaskStatus.Done || status == StartTaskStatus.CanResume || status == StartTaskStatus.UnknownError)
                {
                    m_WaitingTasks.Remove(current);
                }

                if (status == StartTaskStatus.Done || status == StartTaskStatus.UnknownError)
                {
                    ReferencePool.Release(task);
                }

                current = next;
            }
        }

        /// <summary>
        /// 关闭并清理任务池
        /// </summary>
        public void Shutdown()
        {
            RemoveAllTasks();

            while (FreeAgentsCount > 0)
            {
                m_FreeAgents.Pop().Shutdown();
            }
        }

        /// <summary>
        /// 移除所有任务
        /// </summary>
        /// <returns></returns>
        public int RemoveAllTasks()
        {
            int count = m_WaitingTasks.Count + m_WorkingAgents.Count;

            foreach (T task in m_WaitingTasks)
            {
                ReferencePool.Release(task);
            }
            
            m_WaitingTasks.Clear();

            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                T task = workingAgent.Task;
                workingAgent.Reset();
                m_FreeAgents.Push(workingAgent);
                ReferencePool.Release(task);
            }
            
            m_WorkingAgents.Clear();
            
            return count;
        }

        /// <summary>
        /// 增加任务代理
        /// </summary>
        /// <param name="agent">要增加的任务代理</param>
        public void AddAgent(ITaskAgent<T> agent)
        {
            if (agent == null)
            {
                throw new OSFrameworkException("Task agent is invalid.");
            }
            
            agent.Initialize();
            m_FreeAgents.Push(agent);
        }

        /// <summary>
        /// 根据任务的序列编号获取任务的信息
        /// </summary>
        /// <param name="serialId">要获取信息的任务序列编号</param>
        /// <returns>任务的信息</returns>
        public TaskInfo GetTaskInfo(int serialId)
        {
            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                T workingTask = workingAgent.Task;
                if (workingTask.SerialId == serialId)
                {
                    return new TaskInfo(workingTask.SerialId, workingTask.Tag, workingTask.Proirity,
                        workingTask.UserData, workingTask.Done ? TaskStatus.Done : TaskStatus.Doing,
                        workingTask.Description);
                }
            }

            foreach (T waitingTask in m_WaitingTasks)
            {
                if (waitingTask.SerialId == serialId)
                {
                    return new TaskInfo(waitingTask.SerialId, waitingTask.Tag, waitingTask.Proirity,
                        waitingTask.UserData, TaskStatus.Todo, waitingTask.Description);
                }
            }

            return default(TaskInfo);
        }
        
        /// <summary>
        /// 根据任务标签获取任务信息
        /// </summary>
        /// <param name="tag">要获取信息的任务标签</param>
        /// <returns>任务信息</returns>
        public TaskInfo[] GetTaskInfos(string tag)
        {
            List<TaskInfo> results = new List<TaskInfo>();
            GetTaskInfos(tag, results);
            return results.ToArray();
        }
        
        /// <summary>
        /// 根据任务标签获取任务信息
        /// </summary>
        /// <param name="tag">任务标签</param>
        /// <param name="results">任务信息</param>
        public void GetTaskInfos(string tag, List<TaskInfo> results)
        {
            if (results == null)
            {
                throw new OSFrameworkException("Results is invalid.");
            }
            
            results.Clear();
            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                T workingTask = workingAgent.Task;
                if (workingTask.Tag == tag)
                {
                    results.Add(new TaskInfo(workingTask.SerialId, workingTask.Tag, workingTask.Proirity,
                        workingTask.UserData, workingTask.Done ? TaskStatus.Done : TaskStatus.Doing,
                        workingTask.Description));
                }
            }

            foreach (T waitingTask in m_WaitingTasks)
            {
                if (waitingTask.Tag == tag)
                {
                    results.Add(new TaskInfo(waitingTask.SerialId, waitingTask.Tag, waitingTask.Proirity,
                        waitingTask.UserData, TaskStatus.Todo, waitingTask.Description));
                }
            }
        }

        /// <summary>
        /// 获取所有的任务信息
        /// </summary>
        /// <returns>所有的任务信息</returns>
        public TaskInfo[] GetAllTaskInfos()
        {
            int index = 0;
            TaskInfo[] results = new TaskInfo[m_WorkingAgents.Count + m_WaitingTasks.Count];
            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                T workingTask = workingAgent.Task;
                results[index++] = new TaskInfo(workingTask.SerialId, workingTask.Tag, workingTask.Proirity,
                    workingTask.UserData, workingTask.Done ? TaskStatus.Done : TaskStatus.Doing,
                    workingTask.Description);
            }

            foreach (T waitingTask in m_WaitingTasks)
            {
                results[index++] = new TaskInfo(waitingTask.SerialId, waitingTask.Tag, waitingTask.Proirity,
                    waitingTask.UserData, TaskStatus.Todo, waitingTask.Description);
            }

            return results;
        }
        
        /// <summary>
        /// 获取所有任务信息
        /// </summary>
        /// <param name="results"></param>
        public void GetAllTaskInfos(List<TaskInfo> results)
        {
            if (results == null)
            {
                throw new OSFrameworkException("Results is invalid.");
            }
            
            results.Clear();
            foreach(ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                T workingTask = workingAgent.Task;
                results.Add(new TaskInfo(workingTask.SerialId, workingTask.Tag, workingTask.Proirity,
                    workingTask.UserData, workingTask.Done ? TaskStatus.Done : TaskStatus.Doing,
                    workingTask.Description));
            }
            
            foreach (T waitingTask in m_WaitingTasks)
            {
                results.Add(new TaskInfo(waitingTask.SerialId, waitingTask.Tag, waitingTask.Proirity,
                    waitingTask.UserData, TaskStatus.Todo, waitingTask.Description));
            }
        }

        /// <summary>
        /// 增加任务
        /// </summary>
        /// <param name="task">要增加的任务</param>
        public void AddTask(T task)
        {
            LinkedListNode<T> current = m_WaitingTasks.Last;
            while (current != null)
            {
                if (task.Proirity <= current.Value.Proirity)
                {
                    break;
                }

                current = current.Previous;
            }

            if (current != null)
            {
                m_WaitingTasks.AddAfter(current, task);
            }
            else
            {
                m_WaitingTasks.AddFirst(task);
            }
        }
        
        /// <summary>
        /// 根据任务序列编号移除任务
        /// </summary>
        /// <param name="serialId">要移除任务的序列编号</param>
        /// <returns>是否移除任务成功</returns>
        public bool RemoveTask(int serialId)
        {
            foreach (T waitingTask in m_WaitingTasks)
            {
                if (waitingTask.SerialId == serialId)
                {
                    m_WaitingTasks.Remove(waitingTask);
                    ReferencePool.Release(waitingTask);
                    return true;
                }
            }
            
            // 由于这里有Remove操作，但是LinkedListNode的Agent可能会出现一致的情况（即不用的任务用的是同一个Agent），需要根据Task的SerialId来判断具体删除哪个节点
            LinkedListNode<ITaskAgent<T>> currentWorkingAgent = m_WorkingAgents.First;
            while (currentWorkingAgent != null)
            {
                LinkedListNode<ITaskAgent<T>> nextWorkingAgent = currentWorkingAgent.Next;
                ITaskAgent<T> workingAgent = currentWorkingAgent.Value;
                T workingTask = workingAgent.Task;
                if (workingTask.SerialId == serialId)
                {
                    workingAgent.Reset();
                    m_FreeAgents.Push(workingAgent);
                    m_WorkingAgents.Remove(currentWorkingAgent);
                    ReferencePool.Release(workingTask);
                    return true;
                }

                currentWorkingAgent = nextWorkingAgent;
            }

            return false;
        }
        
        /// <summary>
        /// 根据任务标签移除任务
        /// </summary>
        /// <param name="tag">要移除的任务标签</param>
        /// <returns>移除任务的数量</returns>
        public int RemoveTasks(string tag)
        {
            int count = 0;

            LinkedListNode<T> currentWaitingTask = m_WaitingTasks.First;
            while (currentWaitingTask != null)
            {
                LinkedListNode<T> nextWaitingTask = currentWaitingTask.Next;
                T waitingTask = currentWaitingTask.Value;
                if (waitingTask.Tag == tag)
                {
                    m_WaitingTasks.Remove(currentWaitingTask);
                    ReferencePool.Release(waitingTask);
                    count++;
                }

                currentWaitingTask = nextWaitingTask;
            }
            
            LinkedListNode<ITaskAgent<T>> currentWorkingAgent = m_WorkingAgents.First;
            while (currentWorkingAgent != null)
            {
                LinkedListNode<ITaskAgent<T>> nextWorkingAgent = currentWorkingAgent.Next;
                ITaskAgent<T> workingAgent = currentWorkingAgent.Value;
                T workingTask = workingAgent.Task;
                if (workingTask.Tag == tag)
                {
                    workingAgent.Reset();
                    m_FreeAgents.Push(workingAgent);
                    m_WorkingAgents.Remove(currentWorkingAgent);
                    ReferencePool.Release(workingTask);
                    count++;
                }

                currentWorkingAgent = nextWorkingAgent;
            }

            return count;
        }
        
    }
}