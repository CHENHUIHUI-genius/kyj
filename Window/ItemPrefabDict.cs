// ItemPrefabDict.cs
using UnityEngine;
using System.Collections.Generic;

public class ItemPrefabDict : Singleton<ItemPrefabDict>
{
    [System.Serializable]
    public class ItemPrefabPair
    {
        public ItemName itemName;
        public GameObject prefab;
    }

    public List<ItemPrefabPair> itemPrefabs;
    private Dictionary<ItemName, GameObject> prefabDict = new Dictionary<ItemName, GameObject>();

    private void Start()
    {
        foreach (var pair in itemPrefabs)
        {
            prefabDict[pair.itemName] = pair.prefab;
        }
    }

    public GameObject GetPrefab(ItemName itemName)
    {
        if (prefabDict.TryGetValue(itemName, out var prefab))
            return prefab;
        Debug.LogError($"未找到物品 {itemName} 的预制体");
        return null;
    }
}