using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.Threading;

namespace Arya.Threading
{
    /// <summary>
    /// A class designed to make threading safer and easier to manage.
    /// </summary>
    public abstract class PulseThread
    {
        public bool Stop { get { lock (locker) { return stop; } } }
        public bool Stopped { get { lock (locker) { return stopped; } } }
        public delegate void ThreadStoppedEventDelegate(PulseThread t);
        public event ThreadStoppedEventDelegate ThreadStopped;
        public string Name { get; private set; }

        protected int PulseBreak;

        public Thread Thread { get; private set; }
        private readonly object locker = new object();
        private bool stop;
        private bool stopped;

        /// <summary>
        /// Creates a ManagedThread with the default sleep time between pulses of 100 ms.
        /// </summary>
        public PulseThread(string name)
        {
            PulseBreak = 100;
            stop = false;
            stopped = true;
            Thread = null;
            Name = name;
        }
        /// <summary>
        /// Creates a ManagedThread.
        /// </summary>
        /// <param name="pulseBreak">time to sleep in between pulses</param>
        public PulseThread(string name, int pulseBreak)
        {
            PulseBreak = pulseBreak;
            stop = false;
            stopped = true;
            Thread = null;
            Name = name;
        }

        public void Start()
        {
            if (Thread != null)
                throw new Exception("PulseThread already running, cannot start until it has stopped.");
            Thread = new Thread(new ParameterizedThreadStart(ThreadExecuteCommands));
            stopped = true;
            stop = false;
            Thread.Start();
        }

        /// <summary>
        /// Causes the thread to sleep.
        /// </summary>
        /// <param name="milliseconds">time in milliseconds to sleep</param>
        protected void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Signals the thread to stop execution.
        /// </summary>
        public void SetStop()
        {
            lock (locker)
                stop = true;
        }

        public void Join()
        {
            Thread.Join();
        }
        public void Join(int milliseconds)
        {
            Thread.Join(milliseconds);
        }
        public void Join(TimeSpan timeout)
        {
            Thread.Join(timeout);
        }

        private void SetStopped()
        {
            lock (locker)
                stopped = true;

            // Taking the snapshot ensures that the event is not nullified before it is called.
            ThreadStoppedEventDelegate snapshot;
            lock (this)
                snapshot = ThreadStopped;
            if(snapshot!=null)
                ThreadStopped(this);
        }

        private void ThreadExecuteCommands(object o)
        {
            OnStart();
            while (!Stop && Pulse())
            {
                Sleep(100);
            }
            OnStop();
            SetStopped();
        }

        /// <summary>
        /// Called before the thread begins pulsing.
        /// </summary>
        abstract protected void OnStart();
        
        /// <summary>
        /// The pulse method is called by the thread loop, followed by a sleep time determined by PulseBreak.
        /// </summary>
        /// <returns>false to quit the thread loop, true to continue after sleep time</returns>
        abstract protected bool Pulse();

        /// <summary>
        /// TCalled just before the thread exits.
        /// </summary>
        abstract protected void OnStop();
    }
}
