using System;
using Xenko.Core.Mathematics;
using Xenko.Input;
using Xenko.Engine;
using Xenko.Games.Time;
using Xenko.Graphics;
using Xenko.Rendering;
using Xenko.Audio;

namespace Xenko_GameOff2018
{
    public class Timer : SyncScript
    {
        TimerTick timerTick = new TimerTick();

        float amount = 0;
        bool paused;

        public Timer(float amount)
        {
            this.amount = amount;
        }

        public Timer()
        {

        }

        public float Seconds
        {
            get { return (float)timerTick.TotalTime.TotalSeconds; }
        }

        public float Amount
        {
            get=> amount;

            set
            {
                Reset(value);
            }
        }

        public bool Expired
        {
            get
            {
                return timerTick.TotalTime.TotalSeconds > amount;
            }
        }

        public bool Paused { get => paused; }

        public override void Update()
        {
            if (!paused)
                timerTick.Tick();
        }

        public void SetPause(bool pause)
        {
            paused = pause;

            if (paused)
            {
                timerTick.Pause();
            }
            else
            {
                timerTick.Resume();
            }
        }

        public void Reset(float length)
        {
            timerTick.Reset();
            amount = length;
        }

        public void Reset()
        {
            timerTick.Reset();
        }
    }
}
