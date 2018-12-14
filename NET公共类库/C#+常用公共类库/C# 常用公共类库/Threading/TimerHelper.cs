﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// Provides the definition to create timers within their own threads and within critical sections.
    /// </summary>
    [Serializable]
    public class TimerHelper : System.ComponentModel.Component
    {
        private System.Threading.Timer timer;
        private long timerInterval;
        private TimerState timerState;

        /// <summary>
        /// The function prototype for timer executions.
        /// </summary>
        public delegate void TimerExecution();

        /// <summary>
        /// Called when the timer is executed.
        /// </summary>
        public event TimerExecution Execute;

        /// <summary>
        /// Creates a timer with a specified interval, and starts after the specified delay.
        /// </summary>
        public TimerHelper()
        {
            timerInterval = 100;
            timerState = TimerState.Stopped;
            timer = new System.Threading.Timer(new TimerCallback(Tick), null, Timeout.Infinite, timerInterval);
        }


        /// <summary>
        /// Creates a timer with a specified interval, and starts after the specified delay.
        /// </summary>
        /// <param name="interval">Interval in milliseconds at which the timer will execute.</param>
        /// <param name="startDelay"></param>
        public TimerHelper(long interval, int startDelay)
        {
            timerInterval = interval;
            timerState = (startDelay == Timeout.Infinite) ? TimerState.Stopped : TimerState.Running;
            timer = new System.Threading.Timer(new TimerCallback(Tick), null, startDelay, interval);
        }

        /// <summary>
        /// Creates a timer with a specified interval.
        /// </summary>
        /// <param name="interval"></param>
        public TimerHelper(long interval, bool start)
        {
            timerInterval = interval;
            timerState = (!start) ? TimerState.Stopped : TimerState.Running;
            timer = new System.Threading.Timer(new TimerCallback(Tick), null, 0, interval);
        }

        /// <summary>
        /// Starts the timer with a specified delay.
        /// </summary>
        /// <param name="delayBeforeStart"></param>
        public void Start(int delayBeforeStart)
        {
            timerState = TimerState.Running;
            timer.Change(delayBeforeStart, timerInterval);
        }

        /// <summary>
        /// Starts the timer instantly.
        /// </summary>
        public void Start()
        {
            timerState = TimerState.Running;
            timer.Change(0, timerInterval);
        }

        /// <summary>
        /// Pauses the timer.
        /// Note: Running threads won't be closed.
        /// </summary>
        public void Pause()
        {
            timerState = TimerState.Paused;
            timer.Change(Timeout.Infinite, timerInterval);
        }

        /// <summary>
        /// Stops the timer.
        /// Note: Running threads won't be closed.
        /// </summary>
        public void Stop()
        {
            timerState = TimerState.Stopped;
            timer.Change(Timeout.Infinite, timerInterval);
        }

        public void Tick(object obj)
        {
            if (timerState == TimerState.Running && Execute != null)
            {
                lock (this)
                {
                    Execute();
                }
            }
        }

        /// <summary>
        /// Gets the state of the timer.
        /// </summary>
        public TimerState State
        {
            get
            {
                return timerState;
            }
        }

        /// <summary>
        /// Gets or sets the timer interval.
        /// </summary>
        public long Interval
        {
            get
            {
                return timerInterval;
            }
            set
            {
                timer.Change(((timerState == TimerState.Running) ? value : Timeout.Infinite), value);
            }
        }
    }

    public enum TimerState
    {
        Stopped,
        Running,
        Paused
    }
}
