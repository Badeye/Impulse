﻿using Sliders.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Sliders.UI
{
    public class UITimer : MonoBehaviour
    {
        public static UITimer _instance;
        public Text textSec;
        public Text textMil;

        private void Start()
        {
            _instance = this;
        }

        // Use this for initialization
        public static void Run()
        {
            Timer.Start(_instance, _instance.textSec, _instance.textMil);
        }

        public static void Pause()
        {
            Timer.Pause();
        }

        public static void Continue()
        {
            Timer.Continue();
        }

        public static void Stop()
        {
            Timer.Stop();
        }

        public static void Reset()
        {
            Timer.Reset();
        }

        public static double GetTime()
        {
            //maybe change passedTime to pauseTime
            double time = Timer.passedTime;
            return time;
        }
    }
}