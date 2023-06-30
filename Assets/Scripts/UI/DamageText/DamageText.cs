using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText
{
   public class DamageText : MonoBehaviour
   {
      [SerializeField] TMP_Text damageText = null;

      public void DestroyText()
      {
         Destroy(gameObject);
      }

      public void SetValue(float amount)
      {
         damageText.SetText(string.Format("{0:0}", amount));
      }
   }
}
