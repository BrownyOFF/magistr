using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class TokenCount : MonoBehaviour
{
    public int EstimateTokens(string text)
    {
        // Умовно кожне слово або група символів вважається за один токен
        var words = text.Split(new char[] { ' ', '\n', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
        int tokenCount = 0;

        foreach (var word in words)
        {
            tokenCount += Encoding.UTF8.GetByteCount(word) / 4; // Оцінка: середній токен має 4 байти
        }
        return tokenCount;
    }

    public int CountTokensInConversation(List<Dictionary<string, string>> conversationHistory)
    {
        int totalTokens = 0;
        foreach (var message in conversationHistory)
        {
            totalTokens += EstimateTokens(message["role"]);
            totalTokens += EstimateTokens(message["content"]);
            totalTokens += 4; // Для службових токенів API
        }
        return totalTokens;
    }
    
    public void TrimConversationHistory(List<Dictionary<string, string>> conversationHistory, int maxTokens)
    {
        while (CountTokensInConversation(conversationHistory) > maxTokens && conversationHistory.Count > 2)
        {
            conversationHistory.RemoveAt(1); // Видаляємо другий запис, залишаючи перший
        }
    }
}
