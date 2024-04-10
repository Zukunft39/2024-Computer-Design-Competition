using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviourSingleton<TutorialPanel>
{
   [SerializeField] private GameObject tutoriaPanel;
   [SerializeField] private Mask mask;
   private static bool flag=true;
   
    void ShowPanel()
   {
      tutoriaPanel.gameObject.SetActive(true);
   }
   
   public void ClosePanel()
   {
      SoundManager.Instance.PlaySFX("btnClick");
      tutoriaPanel.gameObject.SetActive(false);
      flag = false;
      mask.FadeOut();
   }

   public class WaitForTutorialEnd : CustomYieldInstruction
   {
      public WaitForTutorialEnd()
      {
         TutorialPanel.Instance.ShowPanel();
      }
      public override bool keepWaiting
      {
         get
         {
            return flag;
         }
      }
   }
}
