using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Moq;
using Newtonsoft.Json; // Для моків

[TestFixture]
public class SaveLoadScriptTests
{
    private SaveLoadScript saveLoadScript;
    private Mock<ShowMessage> mockShowMessage;
    private Mock<SendRequest> mockSendRequest;
    private Mock<ImageSendRequest> mockImageSendRequest;
    private string testFilePath;
    
    [SetUp]
    public void SetUp()
    {
        // Моки для залежностей
        mockShowMessage = new Mock<ShowMessage>();
        mockSendRequest = new Mock<SendRequest>();
        mockImageSendRequest = new Mock<ImageSendRequest>();

        // Створення тестового об'єкта
        GameObject testObject = new GameObject();
        saveLoadScript = testObject.AddComponent<SaveLoadScript>();
        saveLoadScript.showMessage = mockShowMessage.Object;
        saveLoadScript.sendRequest = mockSendRequest.Object;
        saveLoadScript.imageSendRequest = mockImageSendRequest.Object;

        // Шлях до тестового файлу
        testFilePath = Path.Combine(Application.persistentDataPath, "TestWorld.json");
    }

    [TearDown]
    public void TearDown()
    {
        // Видалення тестового файлу після виконання тестів
        if (File.Exists(testFilePath))
        {
            File.Delete(testFilePath);
        }
    }

    [Test]
    public void SaveConversationHistoryToFile_ShouldSaveCorrectly()
    {
        // Arrange
        var testConversationHistory = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "role", "user" }, { "content", "Hello World!" } },
            new Dictionary<string, string> { { "role", "assistant" }, { "content", "Hi there!" } }
        };

        // Act
        saveLoadScript.SaveConversationHistoryToFile(testConversationHistory, "TestWorld");

        // Assert
        Assert.IsTrue(File.Exists(testFilePath), "Файл не був створений.");
        string fileContent = File.ReadAllText(testFilePath);
        var deserializedData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(fileContent);
        Assert.AreEqual(testConversationHistory.Count, deserializedData.Count, "Кількість повідомлень не співпадає.");
        Assert.AreEqual(testConversationHistory[0]["content"], deserializedData[0]["content"], "Зміст першого повідомлення не співпадає.");
    }

    [Test]
    public void LoadConversationHistoryFromFile_ShouldLoadCorrectly()
    {
        // Arrange
        var testConversationHistory = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string> { { "role", "user" }, { "content", "Hello World!" } },
            new Dictionary<string, string> { { "role", "assistant" }, { "content", "Hi there!" } }
        };
        File.WriteAllText(testFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(testConversationHistory));

        // Act
        var loadedConversationHistory = saveLoadScript.LoadConversationHistoryFromFile(testFilePath);

        // Assert
        Assert.AreEqual(testConversationHistory.Count, loadedConversationHistory.Count, "Кількість завантажених повідомлень не співпадає.");
        Assert.AreEqual(testConversationHistory[0]["content"], loadedConversationHistory[0]["content"], "Зміст першого повідомлення не співпадає.");
    }

    [Test]
    public void LoadConversationHistoryFromFile_ShouldReturnEmpty_WhenFileDoesNotExist()
    {
        // Arrange
        string nonExistentFilePath = Path.Combine(Application.persistentDataPath, "NonExistent.json");

        // Act
        var result = saveLoadScript.LoadConversationHistoryFromFile(nonExistentFilePath);

        // Assert
        Assert.IsNotNull(result, "Результат не повинен бути null.");
        Assert.IsEmpty(result, "Історія розмови повинна бути порожньою.");
    }

    [Test]
    public void CleanBttn_ShouldCleanAllButtons()
    {
        // Arrange
        GameObject button1 = new GameObject();
        button1.tag = "loadbttn";
        GameObject button2 = new GameObject();
        button2.tag = "loadbttn";
        saveLoadScript.arr = new List<GameObject>() { button1, button2 };

        // Act
        saveLoadScript.CleanBttn();

        // Assert
        Assert.AreEqual(0, saveLoadScript.arr.Count, "Кнопки не були очищені.");
    }
    
}
