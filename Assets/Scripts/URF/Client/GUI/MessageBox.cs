namespace URF.Client.GUI {
  using System.Collections.Generic;
  using System.Text;
  using TMPro;
  using UnityEngine;

  /// <summary>
  /// GUI Component that displays a fixed number of messages. This component renders messages top
  /// down by oldest first.
  /// </summary>
  public class MessageBox : MonoBehaviour {

    [SerializeField]
    private int messageLimit = 30;

    private Queue<string> messages;

    private TMP_Text messageText;

    public int MessageLimit {
      get => this.messageLimit;
      set {
        if (value < 0) {
          value = 0;
        }
        this.messageLimit = value;
        this.PruneMessages();
      }
    }

    private void PruneMessages() {
      if (this.messages.Count <= this.MessageLimit) {
        return;
      }
      while (this.messages.Count > this.MessageLimit) {
        _ = this.messages.Dequeue();
      }

      StringBuilder sb = new();
      foreach (string messageToWrite in this.messages) {
        _ = sb.Append(messageToWrite + "\n");
      }
      this.messageText.text = sb.ToString();

    }

    public void Awake() {
      this.messages = new Queue<string>(this.MessageLimit);
      this.messageText = this.gameObject.GetComponent<TMP_Text>();
      this.messageText.text = "";
    }

    /// <summary>
    /// Add a message to the message box.
    /// </summary>
    /// <param name="message">the message to add</param>
    public void AddMessage(string message) {
      while (this.messages.Count >= this.MessageLimit) {
        _ = this.messages.Dequeue();
      }

      this.messages.Enqueue(message);

      this.messageText.text = "";
      StringBuilder sb = new();
      foreach (string messageToWrite in this.messages) {
        _ = sb.Append(messageToWrite + "\n");
      }
      this.messageText.text = sb.ToString();
    }
  }
}
