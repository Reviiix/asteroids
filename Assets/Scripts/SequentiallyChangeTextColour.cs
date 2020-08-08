using System.Collections;
using TMPro;
using UnityEngine;

public class SequentiallyChangeTextColour : MonoBehaviour
{
   private static Coroutine _changeTextRoutine;
   private const int FlashTime = 1;
   private readonly WaitForSeconds _waitTimeBetweenSpawningObstacles = new WaitForSeconds(FlashTime);
   private TMP_Text _textToChange;
   private readonly Color _firstColor = Color.white;
   private readonly Color _secondColor = Color.red;

   private void Awake()
   {
      _textToChange = GetComponent<TMP_Text>();
   }

   private void OnEnable()
   {
      StartChangeTextColorSequence();
   }
   
   private void OnDisable()
   {
      StopChangeTextColorSequence();
   }

   private void StartChangeTextColorSequence()
   {
      _changeTextRoutine = StartCoroutine(ChangeTextColor());
   }
   
   private void StopChangeTextColorSequence()
   {
      if (_changeTextRoutine != null)
      {
         StopCoroutine(_changeTextRoutine);
      }
   }
   
   private IEnumerator ChangeTextColor()
   {
      _textToChange.color = _firstColor;
      yield return _waitTimeBetweenSpawningObstacles;
      _textToChange.color = _secondColor;
      yield return _waitTimeBetweenSpawningObstacles;
      StartChangeTextColorSequence();
   }
}
