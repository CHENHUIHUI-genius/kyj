using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// 场景过渡管理器（单例模式）
/// 核心职责：处理场景间的切换逻辑，包含淡入淡出过渡动画、场景加载/卸载、存档/读档时的场景恢复
/// 实现接口：ISavable（存档接口），支持场景状态的保存与恢复
/// 依赖：EventHandler（全局事件）、Singleton（单例基类）
/// </summary>
//public class TransitionManager : Singleton<TransitionManager>, ISavable                                           // 取消注释
public class TransitionManager : Singleton<TransitionManager>                                                       // 删除
{
    /// <summary> 新游戏初始加载的场景名称（SceneName特性：编辑器中仅显示有效场景名，避免手动输入错误） </summary>
    [SceneName] public string startScene;

    /// <summary> 用于淡入淡出的画布组（控制透明度和射线阻挡） </summary>
    public CanvasGroup fadeCanvasGroup;

    /// <summary> 淡入淡出动画的持续时长（秒） </summary>
    public float fadeDuration;

    /// <summary> 标记是否正在执行淡入淡出动画（防止重复触发过渡） </summary>
    private bool isFade;

    private bool shouldLoadSave = true; // 标记是否应该加载存档

    ///// <summary> 标记是否允许执行场景过渡（仅游戏运行状态GamePlay时允许） </summary>
    //private bool canTransition;

    /// <summary>
    /// 脚本激活时订阅全局事件
    /// 保证仅在脚本活跃时监听事件，避免无效响应
    /// </summary>
    private void OnEnable()
    {
        // 订阅游戏状态变更事件：用于控制是否允许场景过渡
        //EventHandler.GameStateChangedEvent += OnGameStateChangedEvent;
        // 订阅开始新游戏事件：触发新游戏的初始场景加载
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.GoBackToMenuEvent += OnGoBackToMenuEvent;
    }

    /// <summary>
    /// 脚本禁用/销毁时取消事件订阅
    /// 核心：避免内存泄漏，防止事件触发时调用已销毁的脚本方法
    /// </summary>
    private void OnDisable()
    {
        //EventHandler.GameStateChangedEvent -= OnGameStateChangedEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.GoBackToMenuEvent -= OnGoBackToMenuEvent;
    }


    private void OnStartNewGameEvent()
    {
        Debug.Log("处理开始新游戏事件");
        // 清除存档并跳转到主场景
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        // 标记为不加载存档
        shouldLoadSave = false;

        Transition("Menu", startScene); // 替换为你的主场景名
    }

    //private void OnGoBackToMenuEvent()
    //{
    //    // 保存当前场景名
    //    PlayerPrefs.SetString("CurrentScene", SceneManager.GetActiveScene().name);
    //    PlayerPrefs.Save();

    //    // 保存游戏状态
    //    SaveLoadManager.Instance?.SaveGame();

    //    // 返回菜单
    //    Transition(SceneManager.GetActiveScene().name, "Menu");
    //}    ///// <summary>

    private void OnGoBackToMenuEvent()
    {
        Debug.Log("处理返回菜单事件");
        // 保存当前场景
        PlayerPrefs.SetString("CurrentScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        // 保存游戏状态
        SaveLoadManager.Instance?.SaveGame();

        // 标记为加载存档（返回菜单后继续游戏）
        shouldLoadSave = true;

        // 返回菜单
        string currentScene = SceneManager.GetActiveScene().name;
        Transition(currentScene, "Menu");
    }

    ///// 响应“开始新游戏”事件的处理函数
    ///// 逻辑：从主菜单（Menu）过渡到新游戏初始场景（startScene）
    ///// </summary>
    //private void OnStartNewGameEvent()
    //{
    //    StartCoroutine(TransitionToScene("Menu", startScene));
    //}

    /// <summary>
    /// 初始化：将自身注册到存档系统（ISavable接口实现）
    /// 确保场景状态能被SaveLoadManager识别并纳入存档/读档流程
    /// </summary>
    //private void Start()
    //{
    //    StartCoroutine(TransitionToScene(string.Empty, startScene));                                // 注释，下行取消注释
    //    //    ISavable savable = this;
    //    //    savable.SavableRegister();
    //}
    private void Start()
    {
        // 只在非菜单场景中才自动跳转
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            StartCoroutine(TransitionToScene(string.Empty, startScene));
        }
    }


    ///// <summary>
    ///// 响应游戏状态变更事件的处理函数
    ///// 核心：仅当游戏状态为GamePlay时，才允许执行场景过渡
    ///// </summary>
    ///// <param name="gameState">当前游戏状态（如Menu/GamePlay/Pause等）</param>
    //private void OnGameStateChangedEvent(GameState gameState)
    //{
    //    // 三元逻辑简化：GamePlay状态返回true（允许过渡），其他状态返回false（禁止过渡）
    //    canTransition = gameState == GameState.GamePlay;
    //}


    /// <summary>
    /// 外部调用的场景过渡入口方法
    /// </summary>
    /// <param name="from">需要卸载的源场景名称（空字符串则不卸载）</param>
    /// <param name="to">需要加载的目标场景名称</param>
    public void Transition(string from, string to)
    {
        // 防重入判断：未在淡入淡出 && 允许过渡 → 才执行场景过渡
        // 避免同一时间多次触发过渡导致场景加载异常
        //if (!isFade && canTransition)                                                                         // 取消注释，下一行注释
        if (!isFade)
            StartCoroutine(TransitionToScene(from, to));
        // 若正在淡入淡出/不允许过渡，则直接跳过，无报错（安全设计）
    }

    /// <summary>
    /// 场景过渡核心协程（异步执行，避免卡顿）
    /// 流程：淡出 → 卸载旧场景 → 加载新场景 → 设为活跃场景 → 触发加载后事件 → 淡入
    /// </summary>
    /// <param name="from">源场景（卸载）</param>
    /// <param name="to">目标场景（加载）</param>
    /// <returns>协程迭代器</returns>
    //private IEnumerator TransitionToScene(string from, string to)
    //{
    //    // 第一步：淡出（画布组透明度设为1，遮挡屏幕，隐藏场景切换过程）
    //    yield return Fade(1);

    //    // 第二步：卸载源场景（若源场景非空）
    //    if (from != string.Empty)
    //    {
    //        // 触发“场景卸载前”事件：通知其他脚本执行清理逻辑（如保存场景数据、销毁临时对象）
    //        EventHandler.CallBeforeSceneUnloadEvent();
    //        // 异步卸载场景：避免同步卸载导致主线程卡顿
    //        yield return SceneManager.UnloadSceneAsync(from);
    //    }

    //    // 第三步：异步加载目标场景（Additive模式：叠加加载，保留当前活跃场景）
    //    yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

    //    // 第四步：将新加载的场景设为活跃场景（确保后续逻辑在新场景执行）
    //    // SceneManager.sceneCount - 1：新加载的场景在场景列表最后一位
    //    Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
    //    SceneManager.SetActiveScene(newScene);

    //    // 第五步：触发“场景加载后”事件：通知其他脚本执行初始化（如同步小游戏通关状态、初始化UI）
    //    EventHandler.CallAfterSceneLoadedEvent();

    //    // 第六步：淡入（画布组透明度设为0，显示新场景）
    //    yield return Fade(0);
    //}
    private IEnumerator TransitionToScene(string from, string to)
    {
        // 第一步：淡出
        yield return Fade(1);

        // 第二步：卸载源场景前保存游戏状态
        if (from != string.Empty)
        {
            SaveLoadManager.Instance?.SaveGame(); // 保存当前场景状态
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return SceneManager.UnloadSceneAsync(from);
        }

        // 第三步：加载目标场景
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

        // 第四步：设为活跃场景
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);

        // 第五步：加载游戏状态（只有shouldLoadSave为true时才加载）
        EventHandler.CallAfterSceneLoadedEvent();
        if (shouldLoadSave)
        {
            SaveLoadManager.Instance?.LoadGame();
        }
        else
        {
            // 重置标记为true，为下次过渡做准备
            shouldLoadSave = true;
        }

        // 第六步：淡入
        yield return Fade(0);
    }


    /// <summary>
    /// 淡入淡出动画协程
    /// 控制CanvasGroup的alpha值实现渐变效果，同时管理射线阻挡（防止动画期间点击UI/场景）
    /// </summary>
    /// <param name="targetAlpha">目标透明度（1=完全不透明/淡出，0=完全透明/淡入）</param>
    /// <returns>协程迭代器</returns>
    private IEnumerator Fade(float targetAlpha)
    {
        // 标记正在淡入淡出：防止重复触发过渡
        isFade = true;
        // 开启射线阻挡：动画期间禁止点击操作（避免误触发）
        fadeCanvasGroup.blocksRaycasts = true;

        // 计算渐变速度：根据当前透明度与目标透明度的差值 / 动画时长，保证速度均匀
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;

        // 渐变循环：直到当前透明度与目标透明度接近（Mathf.Approximately避免浮点精度问题）
        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            // 逐帧更新透明度：MoveTowards保证匀速，避免速度突变
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null; // 等待下一帧，实现平滑动画
        }

        // 动画结束：关闭射线阻挡，恢复交互；标记淡入淡出完成
        fadeCanvasGroup.blocksRaycasts = false;
        isFade = false;
    }

    ///// <summary>
    ///// 实现ISavable接口：生成场景相关的存档数据
    ///// 存档内容：当前活跃场景名称（用于读档时恢复到该场景）
    ///// </summary>
    ///// <returns>包含场景信息的GameSaveData对象</returns>
    //public GameSaveData GenerateSaveData()
    //{
    //    GameSaveData saveData = new GameSaveData();
    //    // 记录当前活跃场景名称，读档时根据该值恢复场景
    //    saveData.currentScene = SceneManager.GetActiveScene().name;
    //    return saveData;
    //}

    ///// <summary>
    ///// 实现ISavable接口：从存档数据恢复场景状态
    ///// 逻辑：从主菜单（Menu）过渡到存档中记录的场景
    ///// </summary>
    ///// <param name="saveData">存档数据（包含要恢复的场景名称）</param>
    //public void RestoreGameData(GameSaveData saveData)
    //{
    //    Transition("Menu", saveData.currentScene);
    //}

}
