using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Statistics
{
    public class TimeTracker : MonoBehaviour
    {
        private static TMP_Text _timeText;
        private static bool _trackTime;
        private static Coroutine _timeTracker;
        private const string TimeDisplayPrefix = "TIME: ";
        private static DateTime _startTime;
        public static TimeSpan finalTime;

        private void Start()
        {
            InitialiseVariables();
        }

        private static void InitialiseVariables()
        {
            _timeText = GameManager.Instance.userInterfaceManager.timeText;
        }

        public void StartTimer()
        {
            _trackTime = true;
            _startTime = DateTime.Now;
            _timeTracker = StartCoroutine(TrackTime(_startTime));
        }
    
        public void StopTimer()
        {
            _trackTime = false;
            OnTimerStop(TrackTimeFrom(_startTime));
        }
    
        private IEnumerator TrackTime(DateTime startingTime)
        {
            while (_trackTime)
            {
                if (_trackTime == false)
                {
                    OnTimerStop(TrackTimeFrom(startingTime));
                    yield return null;
                }
                UpdateTimeDisplay(startingTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnTimerStop(TimeSpan endTime)
        {
            if (_timeTracker != null)
            {
                StopCoroutine(_timeTracker);
            }
            finalTime = endTime;
        }

        private static void UpdateTimeDisplay(DateTime startingTime)
        {
            _timeText.text = TimeDisplayPrefix + TrackTimeFrom(startingTime).ToString(@"m\:ss\:ff");
        }
    
        private static TimeSpan TrackTimeFrom(DateTime timeSoFar)
        {
            return(DateTime.Now - timeSoFar);
        }
    }
}
