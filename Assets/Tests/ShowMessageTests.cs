using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.TestTools;
using Moq;

[TestFixture]
public class ShowMessageTests
{
    private ShowMessage showMessage;
    private GameObject gameObject;
    private GameObject userMessagePrefab;
    private GameObject aiMessagePrefab;
    private Mock<SendRequest> mockSendRequest;
    private SaveLoadScript saveLoadScript;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        showMessage = gameObject.AddComponent<ShowMessage>();

        // Create message prefabs and add MessageScript components
        userMessagePrefab = new GameObject();
        aiMessagePrefab = new GameObject();
        userMessagePrefab.AddComponent<MessageScript>();
        aiMessagePrefab.AddComponent<MessageScript>();

        // Assign to ShowMessage fields
        showMessage.userMessagePrefab = userMessagePrefab;
        showMessage.aiMessagePrefab = aiMessagePrefab;

        // Set up SaveLoadScript (real component, not mocked here)
        saveLoadScript = new GameObject().AddComponent<SaveLoadScript>();
        showMessage.saveLoadScript = saveLoadScript;

        // Create a content GameObject and assign it to ShowMessage
        GameObject contentObject = new GameObject();
        showMessage.content = contentObject.transform;

        // Optionally, add a child to content to avoid the "Transform child out of bounds" error
        var childObject = new GameObject();
        childObject.transform.SetParent(showMessage.content);
    }

    [Test]
    public void DeleteAllMessages_ShouldClearAllMessages()
    {
        // Arrange
        var userMessage = new GameObject();
        var aiMessage = new GameObject();
        showMessage.messagesObjects.Add(userMessage);
        showMessage.messagesObjects.Add(aiMessage);

        // Act
        showMessage.DeleteAllMessages();

        // Assert
        Assert.AreEqual(0, showMessage.messagesObjects.Count);
        Assert.AreEqual(0, saveLoadScript.arr.Count);
    }

    [Test]
    public void AddUserMessage_ShouldInstantiateUserMessage()
    {
        // Arrange
        string testMessage = "User test message";

        // Act
        showMessage.AddUserMessage(testMessage);

        // Assert
        Assert.AreEqual(1, showMessage.messagesObjects.Count);
        Assert.AreEqual(testMessage, showMessage.messagesObjects[0].GetComponent<MessageScript>().messageText);
    }

    [Test]
    public void AddAIMessage_ShouldInstantiateAIMessage()
    {
        // Arrange
        string testMessage = "AI test message";

        // Act
        showMessage.AddAIMessage(testMessage);

        // Assert
        Assert.AreEqual(1, showMessage.messagesObjects.Count);
        Assert.AreEqual(testMessage, showMessage.messagesObjects[0].GetComponent<MessageScript>().messageText);
    }

   
}
