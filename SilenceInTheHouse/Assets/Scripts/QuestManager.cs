using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private Dictionary<string, bool> quests = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateQuest(string questName, bool isCompleted)
    {
        quests[questName] = isCompleted;
        Debug.Log($"Квест '{questName}' обновлён: {isCompleted}");
    }

    public void CompleteQuest(string questName)
    {
        quests[questName] = true;
        Debug.Log($"Квест '{questName}' выполнен!");
    }

    public bool IsQuestCompleted(string questName)
    {
        return quests.ContainsKey(questName) && quests[questName];
    }
}