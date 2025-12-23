using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private List<ISavable> savableList = new List<ISavable>();

    public void Register(ISavable savable)
    {
        if (!savableList.Contains(savable))
        {
            savableList.Add(savable);
        }
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();

        foreach (var savable in savableList)
        {
            savable.GenerateSaveData(saveData); // 传递saveData，让每个savable设置自己的数据
        }

        string json = JsonUtility.ToJson(saveData);
        string path = Application.persistentDataPath + "/save.json";
        File.WriteAllText(path, json);
        Debug.Log($"游戏已保存到: {path}, 数据: {json}");
    }

    //public void LoadGame()
    //{
    //    string path = Application.persistentDataPath + "/save.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

    //        foreach (var savable in savableList)
    //        {
    //            savable.RestoreGameData(saveData);
    //        }
    //        Debug.Log("游戏已加载");
    //    }
    //    else
    //    {
    //        Debug.Log("未找到存档文件");
    //    }
    //}
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            // 清理已销毁的对象引用
            savableList.RemoveAll(s => {
                UnityEngine.Object obj = s as UnityEngine.Object;
                return obj == null || obj.Equals(null);
            });

            Debug.Log($"剩余savable对象: {savableList.Count}");

            foreach (var savable in savableList)
            {
                UnityEngine.Object obj = savable as UnityEngine.Object;
                if (obj != null && !obj.Equals(null))
                {
                    try
                    {
                        savable.RestoreGameData(saveData);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"恢复数据时出错: {e.Message}");
                    }
                }
            }
            Debug.Log("游戏已加载");
        }
        else
        {
            Debug.Log("未找到存档文件");
        }
    }

}
