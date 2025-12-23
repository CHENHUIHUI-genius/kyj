using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Start()
    {
        // 检查继续按钮是否可用（有窗户存档或物品栏存档）
        bool hasWindowSave = PlayerPrefs.HasKey("WindowPuzzle_State");
        bool hasInventorySave = PlayerPrefs.HasKey("Inventory_Items");
        bool hasSave = hasWindowSave || hasInventorySave;
        Debug.Log($"检查存档 - 窗户: {hasWindowSave}, 物品栏: {hasInventorySave}, 总计: {hasSave}");
        // continueButton.interactable = hasSave; // 如果你有continueButton引用
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartGame()
    {
        Debug.Log("点击开始新游戏");
        // 触发开始新游戏事件
        EventHandler.CallStartNewGameEvent();
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueGame()
    {
        Debug.Log("点击继续游戏");
        Debug.Log($"TransitionManager.Instance: {TransitionManager.Instance}");
        if (TransitionManager.Instance != null)
        {
            Debug.Log($"startScene: {TransitionManager.Instance.startScene}");
        }

        // 获取上次保存的场景名
        string targetScene = PlayerPrefs.GetString("CurrentScene", TransitionManager.Instance?.startScene ?? "MainScene");
        Debug.Log($"目标场景: {targetScene}");

        // 检查是否有存档（窗户或物品栏）
        bool hasWindowSave = PlayerPrefs.HasKey("WindowPuzzle_State");
        bool hasInventorySave = PlayerPrefs.HasKey("Inventory_Items");
        bool hasSave = hasWindowSave || hasInventorySave;
        Debug.Log($"检查存档 - 窗户: {hasWindowSave}, 物品栏: {hasInventorySave}, 总计: {hasSave}");

        if (hasSave && TransitionManager.Instance != null)
        {
            Debug.Log($"继续游戏到场景: {targetScene}");
            // 标记为需要加载存档
            TransitionManager.Instance.Transition("Menu", targetScene);
        }
        else
        {
            Debug.Log("没有找到存档或TransitionManager不存在");
        }
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void GoBackToMenu()
    {
        Debug.Log("点击返回菜单按钮");
        // 触发返回菜单事件
        EventHandler.CallGoBackToMenuEvent();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
