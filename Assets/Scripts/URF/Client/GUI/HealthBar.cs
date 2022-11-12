using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
  public Image healthBarImage;
  public int CurrentHealth {get;set;}
  public int MaximumHealth {get;set;}

  public void UpdateHealthBar() {
    healthBarImage.fillAmount = Mathf.Clamp((float)CurrentHealth / (float)MaximumHealth, 0, 1f);
  }
}
