using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace URF.Client.GUI {
  public class MessageBox : MonoBehaviour {

    private Queue<string> _messages;

    private TMP_Text _messageText;

    public void Start() {
      _messages = new Queue<string>(30);
      _messageText = GetComponent<TMP_Text>();
    }

    public void AddMessage(string message) {
      while(_messages.Count >= 30) { _messages.Dequeue(); }

      _messages.Enqueue(message);

      _messageText.text = "";
      StringBuilder sb = new();
      foreach(string messageToWrite in _messages) { sb.Append(messageToWrite + "\n"); }
      _messageText.text = sb.ToString();
    }

  }
}
