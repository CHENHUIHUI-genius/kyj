// 物品栏左右键
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEditor.Overlays;
using UnityEngine;

/// <summary>
/// 物品栏管理器（单例模式 + 存档接口实现）
/// 核心职责：管理游戏内物品的添加/使用/移除，同步更新物品栏UI，支持物品状态的存档/读档
/// 依赖：Singleton（单例基类）、ISavable（存档接口）、EventHandler（全局事件）、ItemDataList_SO（物品配置SO）
/// </summary>
//public class InventoryManager : Singleton<InventoryManager>, ISavable                                               //取消注释
public class InventoryManager : Singleton<InventoryManager>                                                           //删除
{
    /// <summary> 物品配置数据SO（存储所有物品的名称、贴图、描述等静态配置） </summary>
    public ItemDataList_SO itemData;

    /// <summary> 当前物品栏的物品列表（存储物品名称枚举，[SerializeField]允许编辑器调试查看） </summary>
    //[SerializeField] private List<ItemName> itemList = new List<ItemName>();
    
    // 改为固定5个槽位的数组（与UI的slots数量对应）
    [SerializeField] private ItemName[] itemArray = new ItemName[5];

    private void Start()
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            itemArray[i] = ItemName.None;
        }
    }

    /// <summary>
    /// 脚本激活时订阅全局事件（标准事件订阅范式）
    /// 监听物品使用、物品切换、场景加载、新游戏启动等核心事件，保证逻辑联动
    /// </summary>
    private void OnEnable()
    {
        EventHandler.ItemUsedEvent += OnItemUsedEvent;                      // 订阅物品使用事件：处理物品使用后的移除逻辑
    //    EventHandler.StartNewGameEvent += OnStartNewGameEvent;              // 订阅开始新游戏事件：清空物品栏
    }

    /// <summary>
    /// 脚本禁用/销毁时取消事件订阅（核心：防止内存泄漏、空引用异常）
    /// 与OnEnable的订阅逻辑严格一一对应，避免遗漏
    /// </summary>
    private void OnDisable()
    {
        EventHandler.ItemUsedEvent -= OnItemUsedEvent;
        //    EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    ///// <summary>
    ///// 初始化方法（Unity生命周期）
    ///// 核心：将物品栏管理器注册到存档系统，保证物品状态能被存档/读档
    ///// </summary>
    //private void Start()
    //{
    //    ISavable savable = this;
    //    savable.SavableRegister(); // 调用ISavable接口的默认方法，注册到SaveLoadManager
    //}

    ///// <summary>
    ///// 响应“开始新游戏”事件的处理函数
    ///// 逻辑：清空物品栏列表，新游戏从零开始，无任何物品
    ///// </summary>
    //private void OnStartNewGameEvent()
    //{
    //    itemList.Clear();
    //}


    // 使用物品时只清空当前槽位，不移动其他物品
    private void OnItemUsedEvent(ItemName itemName)
    {
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i] == itemName)
            {
                itemArray[i] = ItemName.None; // 标记为空
                EventHandler.CallUpdateUIEvent(null, i); // 通知UI清空该槽位
                return;
            }
        }
    }


    public void AddItem(ItemName itemName)
    {
        // 避免重复添加（如果需要可堆叠可删除此判断）
        if (System.Array.IndexOf(itemArray, itemName) != -1)
            return;

        // 查找第一个空槽位
        for (int i = 0; i < itemArray.Length; i++)
        {
            if (itemArray[i] == ItemName.None)
            {
                itemArray[i] = itemName;
                EventHandler.CallUpdateUIEvent(itemData.GetItemDetails(itemName), i);
                return;
            }
        }
        Debug.LogWarning("物品栏已满");
    }

    // 辅助方法：获取物品所在槽位索引（用于验证）
    private int GetItemIndex(ItemName itemName)
    {
        return System.Array.IndexOf(itemArray, itemName);
    }



    /// <summary>私有工具方法：根据物品名称查找其在列表中的索引  作用：封装索引查询逻辑，避免重复代码</summary>
    /// <param name="itemName">要查找的物品名称（枚举）</param>
    /// <returns>物品索引（存在则返回对应下标，不存在返回-1）</returns>
    //private int GetItemIndex(ItemName itemName)
    //    {
    //        // 遍历物品列表，匹配物品名称
    //        for (int i = 0; i < itemList.Count; i++)
    //        {
    //            if (itemList[i] == itemName)
    //                return i;
    //        }
    //        // 未找到返回-1（异常保护）
    //        return -1;
    //    }

    ///// <summary>
    ///// 实现ISavable接口：生成物品栏存档数据
    ///// 存档内容：当前物品栏的所有物品名称列表
    ///// </summary>
    ///// <returns>包含物品列表的GameSaveData对象</returns>
    //public GameSaveData GenerateSaveData()
    //{
    //    GameSaveData saveData = new GameSaveData();

    //    // 将当前物品列表赋值给存档数据，实现物品状态保存
    //    saveData.itemList = this.itemList;

    //    return saveData;
    //}

    ///// <summary>
    ///// 实现ISavable接口：从存档数据恢复物品栏状态
    ///// 逻辑：将存档中的物品列表赋值给当前物品列表，恢复玩家的物品栏
    ///// </summary>
    ///// <param name="saveData">存档数据（包含物品列表）</param>
    //public void RestoreGameData(GameSaveData saveData)
    //{
    //    this.itemList = saveData.itemList;
    //}

}
