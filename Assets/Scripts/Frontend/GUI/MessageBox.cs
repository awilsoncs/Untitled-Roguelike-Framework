using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class MessageBox : MonoBehaviour
{
    private Queue<string> messages;
    private TMP_Text messageText;

    public void Start() {
        messages = new(30);
        messageText = GetComponent<TMP_Text>();
    }

    public void AddMessage(string message) {
        while (messages.Count >= 30) {
            messages.Dequeue();
        }

        messages.Enqueue(message);

        //var messageEnumerator = messages.AsEnumerable();
        messageText.text = "";
        foreach (var messageToWrite in messages) {
            messageText.text += messageToWrite + "\n";
        }
    }
}
