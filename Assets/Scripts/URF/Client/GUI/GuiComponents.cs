namespace URF.Client.GUI {
  using System;
  using UnityEngine;

  [Serializable]
  public class GuiComponents {

    [SerializeField] private HealthBar healthBar;

    [SerializeField] private MessageBox messageBox;

    public HealthBar HealthBar => this.healthBar;

    public MessageBox MessageBox => this.messageBox;

  }
}
