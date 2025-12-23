using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public List<ItemName> itemList = new List<ItemName>();
    public int windowState;
    public bool shadowDialogueStarted;
    public int completedCount; // 已完成的拼图数量
    public List<int> placedItemIndices = new List<int>(); // 已放置物品的索引
}
