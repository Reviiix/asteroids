using System.Collections;
using TMPro;
using UnityEngine;

public static class SequentiallyChangeTextColour
{
   private static Coroutine _changeTextRoutine;
   private const float FlashTime = 0.75f;
   private static readonly WaitForSeconds WaitTime = new WaitForSeconds(FlashTime);
   private static readonly Color FirstColor = Color.white;
   private static readonly Color SecondColor = Color.black;

   public static void StartChangeTextColorSequence(TMP_Text textToChange)
   {
      _changeTextRoutine = GameManager.instance.StartCoroutine(ChangeTextColor(textToChange));
   }
   
   public static void StopChangeTextColorSequence()
   {
      if (_changeTextRoutine != null)
      {
         GameManager.instance.StopCoroutine(_changeTextRoutine);
      }
   }
   
   private static IEnumerator ChangeTextColor(TMP_Text textToChange)
   {
      textToChange.color = FirstColor;
      yield return WaitTime;
      textToChange.color = SecondColor;
      yield return WaitTime;
      StartChangeTextColorSequence(textToChange);
   }
}
