// 物品栏左右键
using UnityEngine;
using System;

/// <summary>
/// 全局事件管理器（静态类）
/// 核心职责：集中定义游戏所有全局事件，提供统一的事件触发方法，实现跨脚本/跨场景的解耦通信
/// 设计原则：事件按功能分类，命名规范（Event后缀为事件，Call前缀为触发方法），所有事件均做空安全触发
/// </summary>
public static class EventHandler
{
    #region UI相关事件
    /// <summary>
    /// 物品栏UI更新事件
    /// 触发时机：物品添加/移除/切换时，通知UI刷新显示
    /// 参数1：物品详情（ItemDetails）- 要显示的物品数据（null表示清空对应位置）
    /// 参数2：索引（int）- 物品栏位置索引（-1表示清空所有UI）
    /// </summary>
    public static event Action<ItemDetails, int> UpdateUIEvent;

    /// <summary>
    /// 触发物品栏UI更新事件（空安全封装）
    /// </summary>
    /// <param name="itemDetails">物品详情（null=清空）</param>
    /// <param name="index">物品栏索引（-1=清空所有）</param>
    public static void CallUpdateUIEvent(ItemDetails itemDetails, int index)
    {
        // ?.Invoke()：空安全触发，无订阅者时不执行，避免空引用异常
        UpdateUIEvent?.Invoke(itemDetails, index);
        // 用于更新UI的事件调用
    }
    #endregion

    #region 场景相关事件
    /// <summary>
    /// 场景卸载前事件
    /// 触发时机：场景切换前（卸载旧场景），通知各脚本执行收尾逻辑（如保存数据、销毁临时对象）
    /// </summary>
    public static event Action BeforeSceneUnloadEvent;

    /// <summary>
    /// 触发场景卸载前事件
    /// </summary>
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
        // 场景卸载前调用的事件
    }

    /// <summary>
    /// 场景加载后事件
    /// 触发时机：新场景加载完成并设为活跃场景后，通知各脚本执行初始化逻辑（如同步小游戏状态、恢复UI）
    /// </summary>
    public static event Action AfterSceneLoadedEvent;

    /// <summary>
    /// 触发场景加载后事件
    /// </summary>
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
        // 场景加载后调用的事件
    }
    #endregion

    #region 物品相关事件
    /// <summary>
    /// 物品选中事件
    /// 触发时机：玩家选中/取消选中物品栏中的物品时，通知UI/交互系统更新选中状态
    /// 参数1：选中的物品详情（ItemDetails）
    /// 参数2：是否选中（bool）- true=选中，false=取消选中
    /// </summary>
    public static event Action<ItemDetails, bool> ItemSelectedEvent;

    /// <summary>
    /// 触发物品选中事件
    /// </summary>
    /// <param name="itemDetails">选中的物品详情</param>
    /// <param name="isSelected">是否选中</param>
    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<int> SlotClickedEvent;
    //                                                                                                  new
    public static void CallSlotClickedEvent(int index)
    {
        SlotClickedEvent?.Invoke(index);
    }



    /// <summary>
    /// 物品使用事件
    /// 触发时机：玩家成功使用物品（如用钥匙解锁），通知InventoryManager移除物品并更新UI
    /// 参数：使用的物品名称（ItemName枚举）- 标识具体使用的物品
    /// </summary>
    // 在Inventory Manager中移除used Item并update UI
    public static event Action<ItemName> ItemUsedEvent;

    /// <summary>
    /// 触发物品使用事件
    /// </summary>
    /// <param name="itemName">使用的物品名称</param>
    public static void CallItemUsedEvent(ItemName itemName)
    {
        ItemUsedEvent?.Invoke(itemName);
    }

    #endregion

    #region 窗户相关事件
    // 在适当位置添加
    public static event Action PuzzleCompletedEvent; // 新增
    public static void CallPuzzleCompletedEvent() => PuzzleCompletedEvent?.Invoke(); // 新增
    #endregion

    //#region 对话相关事件
    ///// <summary>
    ///// 显示对话事件
    ///// 触发时机：玩家与NPC交互、触发剧情时，通知UI系统显示对话文本
    ///// 参数：对话内容（string）- 要显示的文本
    ///// </summary>
    //public static event Action<string> ShowDialogueEvent;

    ///// <summary>
    ///// 触发显示对话事件
    ///// </summary>
    ///// <param name="dialogue">对话内容</param>
    //public static void CallShowDialogueEvent(string dialogue)
    //{
    //    ShowDialogueEvent?.Invoke(dialogue);
    //}
    //#endregion

    //#region 游戏状态相关事件
    ///// <summary>
    ///// 游戏状态变更事件
    ///// 触发时机：游戏状态切换（如从菜单→游戏、游戏→暂停），通知各系统更新状态（如TransitionManager是否允许场景切换）
    ///// 参数：新的游戏状态（GameState枚举）- 标识当前游戏状态
    ///// </summary>
    //public static event Action<GameState> GameStateChangedEvent;

    ///// <summary>
    ///// 触发游戏状态变更事件
    ///// </summary>
    ///// <param name="gameState">新的游戏状态</param>
    //public static void CallGameStateChangeEvent(GameState gameState)
    //{
    //    GameStateChangedEvent?.Invoke(gameState);
    //}

    ///// <summary>
    ///// 游戏状态检查事件
    ///// 触发时机：玩家完成关键操作后（如匹配所有小球），通知小游戏脚本检查是否达成通关条件
    ///// </summary>
    //public static event Action CheckGameStateEvent;

    ///// <summary>
    ///// 触发游戏状态检查事件
    ///// </summary>
    //public static void CallCheckGameStateEvent()
    //{
    //    CheckGameStateEvent?.Invoke();
    //}


    ///// <summary>
    ///// 游戏通关事件（静态事件）
    ///// 当游戏达成通关条件时触发，用于通知所有订阅者当前通关的游戏名称
    ///// </summary>
    ///// <param name="gameName">通关的游戏名称字符串参数</param>
    //public static event Action<string> GamePassEvent;

    ///// <summary>
    ///// 触发游戏通关事件的静态方法
    ///// 提供安全的事件调用机制，避免事件未订阅时引发空引用异常
    ///// </summary>
    ///// <param name="gameName">需要传递给订阅者的通关游戏名称</param>
    //public static void CallGamePassEvent(string gameName)
    //{
    //    // 空条件运算符(?.) 检查事件是否有订阅者，有则执行Invoke触发事件并传递参数
    //    // 避免直接调用GamePassEvent.Invoke()时因无订阅者导致的NullReferenceException
    //    GamePassEvent?.Invoke(gameName);
    //}
    //#endregion

    ///// <summary>
    ///// 开始新游戏事件（静态无参事件）
    ///// 触发时机：玩家点击“开始新游戏”按钮时，用于通知所有订阅者执行“新游戏初始化”逻辑
    ///// 设计目的：解耦“点击按钮”操作与“新游戏启动”的具体逻辑（如重置数据、加载场景、初始化UI等）
    ///// </summary>
    //public static event Action StartNewGameEvent;

    ///// <summary>
    ///// 触发“开始新游戏”事件的静态封装方法
    ///// 提供安全的事件调用入口，避免外部直接调用 Invoke 引发空引用异常
    ///// </summary>
    //public static void CallStartNewGameEvent()
    //{
    //    // 空条件运算符（?.）：先判断事件是否有订阅者（非null），再执行触发
    //    // 若直接调用 StartNewGameEvent.Invoke()，无订阅者时会抛出 NullReferenceException
    //    // Invoke()：触发所有订阅该事件的方法，执行新游戏初始化逻辑
    //    StartNewGameEvent?.Invoke();
    //}
}
