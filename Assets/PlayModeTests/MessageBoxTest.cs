namespace PlayModeTests {
  using NUnit.Framework;
  using TMPro;
  using UnityEngine;
  using URF.Client.GUI;

  public class MessageBoxTest {

    private GameObject testObject;
    private TMP_Text widget;

    private MessageBox box;


    [SetUp]
    public void SetUp() {
      this.testObject = new GameObject("TestObject");
      this.widget = this.testObject.AddComponent<TextMeshProUGUI>();
      this.box = this.testObject.AddComponent<MessageBox>();
      this.testObject.SetActive(true);
    }

    [Test]
    public void MessageBox_Should_BeginEmpty() {
      Assert.That(this.widget.text, Is.EqualTo(""));
    }

    [Test]
    public void MessageBox_Should_PopulateOneLineAtATime() {
      this.box.AddMessage("A");
      Assert.That(this.widget.text, Is.EqualTo("A\n"));
    }


    [Test]
    public void MessageBox_Should_AppendNewMessages() {
      this.box.AddMessage("A");
      this.box.AddMessage("B");
      this.box.AddMessage("C");
      Assert.That(this.widget.text, Is.EqualTo("A\nB\nC\n"));
    }

    [Test]
    public void MessageBox_Should_ClipMessagesOnOverflow() {
      this.box.MessageLimit = 2;
      this.box.AddMessage("A");
      this.box.AddMessage("B");
      this.box.AddMessage("C");
      Assert.That(this.widget.text, Is.EqualTo("B\nC\n"));
    }

    [Test]
    public void MessageBox_Should_ClipMessagesWhenLimitIsReduced() {
      this.box.AddMessage("A");
      this.box.AddMessage("B");
      this.box.AddMessage("C");
      int newLimit = 2;
      this.box.MessageLimit = newLimit;
      Assert.That(this.widget.text, Is.EqualTo("B\nC\n"));
    }


    [Test]
    public void MessageBox_Should_GoToAMinimumOfZeroMessages() {
      // debatable semantics here, perhaps warn or error
      int newLimit = -1;
      this.box.MessageLimit = newLimit;
      Assert.That(this.box.MessageLimit, Is.EqualTo(0));
    }

    [Test]
    public void MessageBox_Should_ReturnTheGivenNonNegativeMessageLimit() {
      this.box.MessageLimit = 0;
      Assert.That(this.box.MessageLimit, Is.EqualTo(0));
      int expectedLimit = 73;
      this.box.MessageLimit = expectedLimit;
      Assert.That(this.box.MessageLimit, Is.EqualTo(expectedLimit));
    }
  }
}
