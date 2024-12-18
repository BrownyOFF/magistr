using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TokenCountTests
{
    private TokenCount tokenCounter;

    [SetUp]
    public void Setup()
    {
        // Ініціалізуємо тестовий об'єкт перед кожним тестом
        var gameObject = new GameObject();
        tokenCounter = gameObject.AddComponent<TokenCount>();
    }

    [Test]
    public void EstimateTokens_ShouldCalculateCorrectly()
    {
        // Arrange
        string text = "Hello world!";

        // Act
        int tokenCount = tokenCounter.EstimateTokens(text);

        // Assert
        Assert.AreEqual(2, tokenCount, "Токени не розраховані коректно.");
    }

    [Test]
    public void CountTokensInConversation_ShouldCalculateCorrectly()
    {
        // Arrange
        var conversationHistory = new List<Dictionary<string, string>>()
        {
            new Dictionary<string, string> { { "role", "system" }, { "content", "This is a test message." } },
            new Dictionary<string, string> { { "role", "user" }, { "content", "Hello world!" } }
        };

        // Act
        int totalTokens = tokenCounter.CountTokensInConversation(conversationHistory);

        // Assert
        int lowerBound = 15; // Нижня межа діапазону
        int upperBound = 17; // Верхня межа діапазону
        Assert.IsTrue(totalTokens >= lowerBound && totalTokens <= upperBound, 
            $"Токени у розмові повинні бути в діапазоні {lowerBound}-{upperBound}, але отримано {totalTokens}.");
    }

    [Test]
    public void TrimConversationHistory_ShouldTrimExcessTokens()
    {
        // Arrange
        var conversationHistory = new List<Dictionary<string, string>>()
        {
            new Dictionary<string, string> { { "role", "system" }, { "content", "This is a test message." } },
            new Dictionary<string, string> { { "role", "user" }, { "content", "Hello world!" } },
            new Dictionary<string, string> { { "role", "assistant" }, { "content", "This is assistant's reply." } },
            new Dictionary<string, string> { { "role", "user" }, { "content", "Additional message." } }
        };

        int maxTokens = 34;

        // Act
        tokenCounter.TrimConversationHistory(conversationHistory, maxTokens);

        // Assert
        Assert.AreEqual(3, conversationHistory.Count, "Розмір історії не був обрізаний коректно.");
    }

    [Test]
    public void TrimConversationHistory_ShouldNotTrimBelowTwoEntries()
    {
        // Arrange
        var conversationHistory = new List<Dictionary<string, string>>()
        {
            new Dictionary<string, string> { { "role", "system" }, { "content", "Message 1" } },
            new Dictionary<string, string> { { "role", "user" }, { "content", "Message 2" } }
        };

        int maxTokens = 1; // Задати неможливо мале обмеження

        // Act
        tokenCounter.TrimConversationHistory(conversationHistory, maxTokens);

        // Assert
        Assert.AreEqual(2, conversationHistory.Count, "Історія не має бути обрізана нижче 2 записів.");
    }
}

