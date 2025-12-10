using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 物体管理器 - 负责管理游戏中物品(Item)和交互物体(Interactive)的状态
/// 实现ISavable接口，支持游戏数据的保存与恢复
/// </summary>
//public class ObjectManager : MonoBehaviour, ISavable                                                      // 取消注释，下一行注释
public class ObjectManager : MonoBehaviour
{

    /// <summary>物品可用性字典 - 记录每个物品(ItemName枚举)是否可用(显示/存在)</summary>
    private Dictionary<ItemName, bool> itemAvailableDict = new Dictionary<ItemName, bool>();


    /// <summary>交互状态字典 - 记录每个交互物体是否完成交互</summary>
    private Dictionary<string, bool> interactiveStateDict = new Dictionary<string, bool>();


    /// <summary>启用时注册事件监听</summary>
    private void OnEnable()
    {
        // 场景卸载前事件 - 用于保存当前场景的物体状态
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        // 场景加载后事件 - 用于恢复物体状态
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        // UI更新事件 - 物品收集后更新物品状态
        EventHandler.UpdateUIEvent += OnUpdateUIEvent;
        //    // 新游戏开始事件 - 清空所有状态数据
        //    EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    /// <summary>
    /// 禁用时取消事件监听（防止内存泄漏）
    /// </summary>
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.UpdateUIEvent -= OnUpdateUIEvent;
        //    EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    ///// <summary>
    ///// 初始化时注册可存档对象
    ///// </summary>
    //private void Start()
    //{
    //    // 将当前对象注册到存档系统中
    //    ISavable savable = this;
    //    savable.SavableRegister();
    //}

    ///// <summary>
    ///// 新游戏开始时的处理逻辑
    ///// </summary>
    //private void OnStartNewGameEvent()
    //{
    //    // 清空物品可用性字典
    //    itemAvailableDict.Clear();
    //    // 清空交互状态字典
    //    interactiveStateDict.Clear();
    //}

    /// <summary>
    /// 场景卸载前保存物体状态
    /// </summary>
    private void OnBeforeSceneUnloadEvent()
    {
        // 遍历场景中所有Item组件，保存其可用性状态
        foreach (var item in FindObjectsByType<Item>(FindObjectsSortMode.None))
        {
            // 如果字典中没有该物品，则添加（默认设为可用）
            if (!itemAvailableDict.ContainsKey(item.itemName))
                itemAvailableDict.Add(item.itemName, true);
        }

        // 遍历场景中所有Interactive组件，保存其交互完成状态
        foreach (var item in FindObjectsByType<Interactive>(FindObjectsSortMode.None))
        {
            // 如果字典中已有该交互物体，更新状态；否则添加新条目
            if (interactiveStateDict.ContainsKey(item.name))
                interactiveStateDict[item.name] = item.isDone;
            else
                interactiveStateDict.Add(item.name, item.isDone);
        }
    }

    /// <summary>
    /// 场景加载后恢复物体状态
    /// </summary>
    private void OnAfterSceneLoadedEvent()
    {
        // 恢复物品状态：遍历所有Item组件，根据字典设置激活状态
        foreach (var item in FindObjectsByType<Item>(FindObjectsSortMode.None))
        {
            // 字典中无该物品则添加（默认可用），否则按字典状态设置物体激活状态
            if (!itemAvailableDict.ContainsKey(item.itemName))
                itemAvailableDict.Add(item.itemName, true);
            else
                item.gameObject.SetActive(itemAvailableDict[item.itemName]);
        }

        // 恢复交互物体状态：遍历所有Interactive组件，根据字典设置完成状态
        foreach (var item in FindObjectsByType<Interactive>(FindObjectsSortMode.None))
        {
            // 字典中有该交互物体则恢复状态，否则添加新条目
            if (interactiveStateDict.ContainsKey(item.name))
                item.isDone = interactiveStateDict[item.name];
            else
                interactiveStateDict.Add(item.name, item.isDone);
        }
    }

    /// <summary>
    /// UI更新时处理物品状态（物品被收集后调用）
    /// </summary>
    /// <param name="itemDetails">物品详情</param>
    /// <param name="arg2">数量（此处未使用）</param>
    private void OnUpdateUIEvent(ItemDetails itemDetails, int arg2)
    {
        // 如果物品详情不为空，将该物品标记为不可用（已收集）
        if (itemDetails != null)
            itemAvailableDict[itemDetails.itemName] = false;
    }

    ///// <summary>
    ///// 生成存档数据（实现ISavable接口）
    ///// </summary>
    ///// <returns>包含物体状态的存档数据对象</returns>
    //public GameSaveData GenerateSaveData()
    //{
    //    GameSaveData saveData = new GameSaveData();
    //    // 将物品状态存入存档数据
    //    saveData.itemAvailableDict = this.itemAvailableDict;
    //    // 将交互状态存入存档数据
    //    saveData.interactiveStateDict = this.interactiveStateDict;

    //    return saveData;
    //}

    ///// <summary>
    ///// 恢复存档数据（实现ISavable接口）
    ///// </summary>
    ///// <param name="saveData">存档数据对象</param>
    //public void RestoreGameData(GameSaveData saveData)
    //{
    //    // 从存档数据恢复物品状态
    //    this.itemAvailableDict = saveData.itemAvailableDict;
    //    // 从存档数据恢复交互状态
    //    this.interactiveStateDict = saveData.interactiveStateDict;
    //}
}