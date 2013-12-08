using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using Arya.Command;
using Arya.Threading;

namespace Arya.Scheduler
{
    public struct PendingTask
    {
        public Task Task;
        public DateTime Time;
    }

    public class TaskScheduler
    {
        public int MaxRunningTasks { get; set; }

        private List<Task> pendingTasks;
        private PulseThreadManager threadManager;
        private List<DateTime> pendingTasksStartTime;
        private Timer timer;
        private int timerInterval;

        /// <summary>
        /// Creates the TaskScheduler with its default maximum running tasks (10).
        /// </summary>
        public TaskScheduler()
        {
            threadManager = new PulseThreadManager();
            pendingTasks = new List<Task>();
            pendingTasksStartTime = new List<DateTime>();
            MaxRunningTasks = 10;
        }
        /// <summary>
        /// Creates the TaskScheduler.
        /// </summary>
        /// <param name="maxRunnningTasks">maximum number of running tasks (threads)</param>
        public TaskScheduler(int maxRunnningTasks)
        {
            threadManager = new PulseThreadManager();
            pendingTasks = new List<Task>();
            MaxRunningTasks = maxRunnningTasks;
        }

        /// <summary>
        /// Starts the task scheduler's timer, which will attempt to execute any pending tasks so long as the task pool is not full.
        /// </summary>
        /// <param name="interval">time between checks to see if any pending tasks should be executed</param>
        public void Start(int interval)
        {
            timer = new Timer();
            timer.Interval = timerInterval = interval;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        /// <summary>
        /// Stops the task scheduler's timer.
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }

        /// <summary>
        /// Adds a task to be executed immediately (if max running threads has not been met).
        /// </summary>
        /// <param name="commands">commands to be executed by the task</param>
        public void AddTask(string[] arguments)
        {
            AddTask(arguments, DateTime.Now);
        }
        /// <summary>
        /// Adds a task to be executed at a specified time (if max running threads has not been met).
        /// </summary>
        /// <param name="commands">commands to be executed by the task</param>
        /// <param name="startTime">time to start the task</param>
        public void AddTask(string[] arguments, DateTime startTime)
        {
            Task nTask = Task.CreateTask(arguments);
            if (nTask != null)
            {
                pendingTasks.Add(nTask);
                pendingTasksStartTime.Add(startTime);
            }
        }

        /// <summary>
        /// Returns a list of of scheduled tasks and their planned start time.
        /// </summary>
        /// <returns>a list of scheduled tasks and their planned start time</returns>
        public List<PendingTask> GetPendingTasks()
        {
            List<PendingTask> tasks = new List<PendingTask>();
            for (int i = 0; i < pendingTasks.Count; i++)
            {
                PendingTask taskToAdd = new PendingTask();
                taskToAdd.Task = pendingTasks[i];
                taskToAdd.Time = DateTime.Now;
                tasks.Add(taskToAdd);
            }
            return tasks;
        }

        /// <summary>
        /// Signals all threads to stop running.
        /// </summary>
        /// <param name="blockUntilStopped">if true the calling thread will block until all threads are stopped</param>
        public void StopTasks(bool blockUntilStopped)
        {
            threadManager.StopThreads(blockUntilStopped);
        }

        // Called at set intervals by the timer. Job is to manage threadpool and start tasks when they are supposed to be started.
        void timer_Tick(object sender, EventArgs e)
        {
            if (pendingTasks.Count > 0)
            {
                int i = 0;
                while(i < pendingTasks.Count && threadManager.Count < MaxRunningTasks)
                {
                    if (pendingTasksStartTime[i] <= DateTime.Now)
                    {
                        threadManager.StartThread(pendingTasks[i]);
                        pendingTasks.RemoveAt(i);
                        pendingTasksStartTime.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
    }
}
