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
        private const string TimeDisplayPrefix = "TIME: ";
        private static DateTime _startTime;

        public static void Initialise()
        {
            _timeText = GameManager.instance.userInterfaceManager.timeText;
        }

        public static void StartTimer()
        {
            _trackTime = true;
            _startTime = DateTime.Now;
            _timeTracker = GameManager.instance.StartCoroutine(TrackTime(_startTime));
        }
    
        public static void StopTimer()
        {
            _trackTime = false;
            OnTimerStop(TrackTimeFrom(_startTime));
        }
    
        private static IEnumerator TrackTime(DateTime startingTime)
        {
            while (_trackTime)
            {
                if (_trackTime == false)
                {
                    OnTimerStop(TrackTimeFrom(startingTime));
                    yield return null;
                }
                UpdateTimeDisplay(_timeText, startingTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private static void OnTimerStop(TimeSpan endTime)
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
    
        private static TimeSpan TrackTimeFrom(DateTime timeSoFar)
        {
            return(DateTime.Now - timeSoFar);
        }
    }
}
