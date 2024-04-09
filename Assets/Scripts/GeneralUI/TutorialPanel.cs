using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviourSingleton<TutorialPanel>
{
   [SerializeField] private GameObject tutoriaPanel;
   private static bool flag=true;
   
    void ShowPanel()
   {
      tutoriaPanel.gameObject.SetActive(true);
   }
   
   public void ClosePanel()
   {
      tutoriaPanel.gameObject.SetActive(false);
      flag = false;
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
