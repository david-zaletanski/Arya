using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arya.Threading
{
    public class PulseThreadManager
    {
        public List<PulseThread> Threads { get; set; }
        public int Count { get { return Threads.Count; } }

        public PulseThreadManager()
        {
            Threads = new List<PulseThread>();
        }

        public PulseThread this[int n]
        {
            get
            {
                return Threads[n];
            }
            set
            {
                Threads[n] = value;
            }
        }

        /// <summary>
        /// Adds a thread to the managed list and starts it.
        /// </summary>
        /// <param name="thread">thread to add to the managed list</param>
        public void StartThread(PulseThread thread)
        {
            thread.ThreadStopped += new PulseThread.ThreadStoppedEventDelegate(thread_ThreadStopped);
            Threads.Add(thread);
            thread.Start();
        }

        // Removes a thread after it is finished executing.
        private void thread_ThreadStopped(PulseThread t)
        {
            Threads.Remove(t);
        }

        /// <summary>
        /// Sets the signal to stop on all threads.
        /// </summary>
        /// <param name="blockUntilStopped">the calling thread will block until all threads exit</param>
        public void StopThreads(bool blockUntilStopped)
        {
            foreach (PulseThread t in Threads)
            {
                t.SetStop();
                if(blockUntilStopped)
                    t.Join();
            }
        }
    }
}
