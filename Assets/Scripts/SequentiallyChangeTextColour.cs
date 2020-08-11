using System.Collections;
using TMPro;
using UnityEngine;

public static class SequentiallyChangeTextColour
{
   private static Coroutine _changeTextRoutine;
   private const float ChangeTime = 0.75f;
   private static readonly WaitForSeconds WaitChangeTime = new WaitForSeconds(ChangeTime);
   private static readonly Color FirstColor = Color.white;
   private static readonly Color SecondColor = Color.black;

   public static void StartChangeTextColorSequence(TMP_Text textToChange)
   {
      _changeTextRoutine = GameManager.instance.StartCoroutine(ChangeTextColorSequence(textToChange));
   }
   
   public static void StopChangeTextColorSequence()
   {
      if (_changeTextRoutine != null)
      {
         GameManager.instance.StopCoroutine(_changeTextRoutine);
      }
   }
   
   private static IEnumerator ChangeTextColorSequence(TMP_Text textToChange)
   {
      textToChange.color = FirstColor;
      yield return WaitChangeTime;
      textToChange.color = SecondColor;
      yield return WaitChangeTime;
      StartChangeTextColorSequence(textToChange);
   }
}
