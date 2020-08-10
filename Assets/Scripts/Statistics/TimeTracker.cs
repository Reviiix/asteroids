using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Statistics
{
    public static class TimeTracker
    {
        private static TMP_Text _timeText;
        private static bool _trackTime;
        private static Coroutine _timeTracker;
        private const string TimeDisplayPrefix = "<u>TIME:</u> ";
        private static DateTime _startTime;

        public static void Initialise()
        {
            _timeText = GameManager.instance.userInterfaceManager.timeText;
            _timeText.text = TimeDisplayPrefix + "0:00:00";
        }

        public static void StartTimer()
        {
            _trackTime = true;
            _startTime = DateTime.Now;
            _timeTracker = GameManager.instance.StartCoroutine(TrackTime(_startTime));
        }
        
        public static void PauseTimer()
        {
            pauseStart = DateTime.Now;
            StopTimer();
        }

        private static DateTime pauseStart;
        private static DateTime pauseEnd;
        private static TimeSpan pauseTime;
        
        public static void ResumeTimer()
        {
            pauseEnd = DateTime.Now;
            pauseTime = pauseEnd - pauseStart;
            
            _trackTime = true;
            _startTime += pauseTime;
            _timeTracker = GameManager.instance.StartCoroutine(TrackTime(_startTime));
        }
        
    
        public static void StopTimer()
        {
            _trackTime = false;
            OnTimerStop();
        }
    
        private static IEnumerator TrackTime(DateTime startingTime)
        {
            while (_trackTime)
            {
                if (_trackTime == false)
                {
                    OnTimerStop();
                    yield return null;
                }
                UpdateTimeDisplay(_timeText, startingTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private static void OnTimerStop()
        {
            if (_timeTracker != null)
            {
                GameManager.instance.StopCoroutine(_timeTracker);
            }
        }

        private static void UpdateTimeDisplay(TMP_Text display, DateTime startingTime)
        {
            display.text = TimeDisplayPrefix + TrackTimeFrom(startingTime).ToString(@"m\:ss\:ff");
        }
    
        private static TimeSpan TrackTimeFrom(DateTime originalTime)
        {
            return(DateTime.Now - originalTime);
        }
    }
}
