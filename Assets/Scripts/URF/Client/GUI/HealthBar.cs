namespace URF.Client.GUI {
  using UnityEngine;
  using UnityEngine.UI;

  public class HealthBar : MonoBehaviour {

    [SerializeField] private Image healthBarImage;

    public int MaximumHealth {
      get; set;
    }

    public void UpdateHealthBar(int currentHealth) {
      this.healthBarImage.fillAmount = Mathf.Clamp(
        (float)currentHealth / (float)this.MaximumHealth, 0, 1f
      );
    }

  }
}
