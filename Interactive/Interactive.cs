using UnityEngine;

/// <summary>
/// 场景交互基类（挂载在可交互对象上，如NPC、机关、道具台等）
/// 核心职责：检测玩家使用的物品是否匹配交互所需物品，触发对应的交互逻辑，管理交互完成状态
/// 设计：采用虚方法+保护方法，支持子类重写自定义交互行为，实现“基类统一逻辑+子类个性化动作”
/// </summary>
public class Interactive : MonoBehaviour
{
    /// <summary> 交互所需的物品名称（枚举） </summary>
    public ItemName requireItem;        // 对应的所需传递的物品

    /// <summary> 交互完成状态标记 </summary>
    public bool isDone;                 // 创建布尔值，判断互动情况是否已经结束


    /// <summary>
    /// 核心检测方法：校验玩家使用的物品是否匹配交互所需物品
    /// 外部调用入口（如玩家点击交互对象+使用物品时触发）
    /// </summary>
    /// <param name="itemName">玩家当前使用的物品名称（枚举）</param>
    public void CheckItem(ItemName itemName)
    {

        // 双重条件校验：1. 玩家使用的物品 == 交互所需物品（物品匹配）；2. 交互未完成（isDone=false，避免重复交互）
        if (itemName == requireItem && !isDone)
        {
            // 标记交互完成，防止重复触发
            isDone = true;

            // 执行交互成功后的自定义动作（子类可重写OnClickedAction实现个性化逻辑）
            OnClickedAction();

            // 触发全局物品使用事件：通知InventoryManager移除该物品（从玩家背包中删除）
            EventHandler.CallItemUsedEvent(itemName);   // 通知全局：该物品已被使用（比如移除玩家背包中的物品）
        }
    }

    /// <summary>
    /// 交互成功后的动作方法（保护虚方法），默认是正确的物品执行
    /// </summary>
    /// 基类空实现：因为不同交互对象的成功动作不同（如解锁、开门、触发对话），由子类重写
    /// protected：仅当前类和子类可访问，外部脚本不能直接调用，保证逻辑封装；
    /// virtual：允许子类用override重写，自定义交互成功后的具体行为
    protected virtual void OnClickedAction()
    {
        // 基类默认无逻辑，子类示例（如DoorInteractive）：
        // gameObject.SetActive(false); // 门解锁后隐藏
        // EventHandler.CallInteractiveDoneEvent(gameObject.name); // 通知全局交互完成
    }

    /// <summary>
    /// 空点击处理方法（玩家未使用物品/使用错误物品点击交互对象时触发）
    /// virtual：允许子类重写自定义提示（如显示“需要钥匙才能解锁”）
    /// </summary>
    public virtual void EmptyClicked()
    {
        // 基类默认打印调试日志，子类可重写为显示UI提示（如弹窗、文字）
        Debug.Log("空点");
    }
}