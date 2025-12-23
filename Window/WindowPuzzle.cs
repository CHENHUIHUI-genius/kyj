using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowPuzzle : Interactive
{
    public Transform[] targetPositions;
    private Draggable[] placedItems = new Draggable[4];
    private int completedCount = 0;
    private System.Collections.Generic.List<Draggable> windowItems = new System.Collections.Generic.List<Draggable>(); // 窗户内的所有物品

    public ItemDataList_SO itemData;
    public Sprite blackWindowSprite;

    // 状态枚举
    private enum WindowState { Normal, PuzzleCompleted, Black, ShadowShown, PostDialogue, Shadow1, Shadow2, Item }
    private WindowState currentState = WindowState.Normal;

    // Shadow相关
    private GameObject shadowObject;
    private ShadowInteractive shadowInteractive;
    private Vector3 shadowOriginalPosition;

    // 保存原始窗户图片
    private Sprite originalWindowSprite;

    private void Awake()
    {
        shadowObject = transform.Find("Shadow").gameObject;
        shadowInteractive = shadowObject.GetComponent<ShadowInteractive>();
        shadowOriginalPosition = shadowObject.transform.position;

        // 保存原始窗户图片
        SpriteRenderer windowRenderer = GetComponent<SpriteRenderer>();
        if (windowRenderer != null)
        {
            originalWindowSprite = windowRenderer.sprite;
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.DialogueFinishedEvent += OnDialogueFinished;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.DialogueFinishedEvent -= OnDialogueFinished;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    // 新游戏开始时重置窗户状态并生成初始物品
    private void OnStartNewGameEvent()
    {
        Debug.Log("窗户谜题 - 新游戏开始，重置状态并生成初始物品");
        // 重置所有状态
        currentState = WindowState.Normal;
        completedCount = 0;
        shadowInteractive.dialogueStarted = false;

        // 清除所有窗户物品
        foreach (var item in windowItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        windowItems.Clear();

        // 重置已放置物品数组
        for (int i = 0; i < placedItems.Length; i++)
        {
            placedItems[i] = null;
        }

        // 生成初始物品（paper1, paper2, paper3, paper4）
        SpawnInitialItems();

        // 重置场景中所有其他物品
        ResetAllSceneItems();

        // 恢复视觉状态
        RestoreVisualState();
    }

    // 重置场景中所有其他物品到初始状态
    private void ResetAllSceneItems()
    {
        Debug.Log("重置场景中所有物品到初始状态");

        // 查找场景中所有的Item对象并销毁（除了窗户区域的物品）
        Item[] allItems = FindObjectsByType<Item>(FindObjectsSortMode.None);
        foreach (Item item in allItems)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
                Debug.Log($"销毁现有物品: {item.itemName}");
            }
        }

        // 查找场景中所有的Box对象并重置状态
        Box[] allBoxes = FindObjectsByType<Box>(FindObjectsSortMode.None);
        foreach (Box box in allBoxes)
        {
            if (box != null)
            {
                // 重置Box状态（如果Box有重置方法）
                // box.ResetToInitialState(); // 如果你添加了这个方法
                Debug.Log($"重置Box: {box.gameObject.name}");
            }
        }

        // 可以在这里添加其他需要重置的对象类型
        // ResetOtherInteractiveObjects();
    }

    // 生成新游戏开始时的初始物品
    private void SpawnInitialItems()
    {
        Debug.Log("生成新游戏初始物品");

        // 生成4个初始paper物品 - 设置固定位置而不是随机
        SpawnItemAtPosition(ItemName.paper1, new Vector2(-6f, 2f));
        SpawnItemAtPosition(ItemName.paper2, new Vector2(-2f, -3f));
        SpawnItemAtPosition(ItemName.paper3, new Vector2(3f, 2f));
        SpawnItemAtPosition(ItemName.paper4, new Vector2(7f, -1f));

        // 可以在这里添加其他需要重置的场景物品
        // 例如：ResetOtherSceneItems();
    }

    // 在指定位置生成物品
    private void SpawnItemAtPosition(ItemName itemName, Vector2 position)
    {
        // 获取物品预制体
        GameObject itemPrefab = ItemPrefabDict.Instance.GetPrefab(itemName);
        if (itemPrefab != null)
        {
            GameObject itemObj = Instantiate(itemPrefab, position, Quaternion.identity);

            // 设置为Item组件（可捡拾）
            Item itemComponent = itemObj.GetComponent<Item>();
            if (itemComponent != null)
            {
                itemComponent.itemName = itemName;
            }

            Debug.Log($"生成初始物品: {itemName} 在位置 {position}");
        }
    }

    // 仿照Box.cs的方式保存和恢复状态
    private void OnAfterSceneLoadedEvent()
    {
        // 检查是否是新游戏（没有窗户存档）
        bool hasWindowSave = PlayerPrefs.HasKey("WindowPuzzle_State");
        Debug.Log($"WindowPuzzle: 开始恢复状态, 有存档: {hasWindowSave}");

        if (!hasWindowSave)
        {
            // 新游戏，直接使用默认状态
            currentState = WindowState.Normal;
            shadowInteractive.dialogueStarted = false;
            completedCount = 0;
            Debug.Log("新游戏，使用默认状态");
        }
        else
        {
            // 从PlayerPrefs恢复状态
            currentState = (WindowState)PlayerPrefs.GetInt("WindowPuzzle_State", 0);
            shadowInteractive.dialogueStarted = PlayerPrefs.GetInt("WindowPuzzle_DialogueStarted", 0) == 1;
            completedCount = PlayerPrefs.GetInt("WindowPuzzle_CompletedCount", 0);

            Debug.Log($"恢复状态 - currentState: {currentState}, dialogueStarted: {shadowInteractive.dialogueStarted}, completedCount: {completedCount}");

            // 恢复窗户内所有物品
            string windowItemsStr = PlayerPrefs.GetString("WindowPuzzle_WindowItems", "");
            Debug.Log($"窗户物品字符串: {windowItemsStr}");

            if (!string.IsNullOrEmpty(windowItemsStr))
            {
                string[] items = windowItemsStr.Split('|');
                foreach (string itemStr in items)
                {
                    string[] parts = itemStr.Split(':');
                    if (parts.Length == 4)
                    {
                        int targetIndex = int.Parse(parts[0]);
                        float posX = float.Parse(parts[1]);
                        float posY = float.Parse(parts[2]);
                        bool isPlaced = int.Parse(parts[3]) == 1;

                        ItemName itemName = GetItemNameByTargetIndex(targetIndex);
                        Debug.Log($"恢复物品: {itemName} 位置({posX},{posY}) 已放置:{isPlaced}");

                        if (itemName != ItemName.None)
                        {
                            GameObject itemPrefab = ItemPrefabDict.Instance.GetPrefab(itemName);
                            if (itemPrefab != null)
                            {
                                Vector3 itemPos = new Vector3(posX, posY, 0);
                                GameObject itemObj = Instantiate(itemPrefab, itemPos, Quaternion.identity);
                                Draggable draggable = itemObj.GetComponent<Draggable>();
                                if (draggable != null)
                                {
                                    draggable.targetIndex = targetIndex;
                                    draggable.isPlaced = isPlaced;
                                    windowItems.Add(draggable);

                                    if (isPlaced && targetIndex >= 0 && targetIndex < placedItems.Length)
                                    {
                                        placedItems[targetIndex] = draggable;
                                    }

                                    // 设置渲染层级
                                    SpriteRenderer sr = itemObj.GetComponent<SpriteRenderer>();
                                    if (sr != null) sr.sortingOrder = isPlaced ? 10 : 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        // 恢复视觉状态
        RestoreVisualState();
        Debug.Log("WindowPuzzle: 状态恢复完成");
    }

    // 保存状态到PlayerPrefs
    private void SaveState()
    {
        Debug.Log($"保存状态到PlayerPrefs - currentState: {currentState}, dialogueStarted: {shadowInteractive.dialogueStarted}, completedCount: {completedCount}");
        PlayerPrefs.SetInt("WindowPuzzle_State", (int)currentState);
        PlayerPrefs.SetInt("WindowPuzzle_DialogueStarted", shadowInteractive.dialogueStarted ? 1 : 0);
        PlayerPrefs.SetInt("WindowPuzzle_CompletedCount", completedCount);

        // 保存窗户内所有物品的信息 (类型:位置X:位置Y:是否已放置)
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var item in windowItems)
        {
            if (item != null)
            {
                string itemData = $"{(int)item.targetIndex}:{item.transform.position.x}:{item.transform.position.y}:{(item.isPlaced ? 1 : 0)}";
                if (sb.Length > 0) sb.Append("|");
                sb.Append(itemData);
            }
        }
        string itemsStr = sb.ToString();
        PlayerPrefs.SetString("WindowPuzzle_WindowItems", itemsStr);
        PlayerPrefs.Save();
        Debug.Log($"PlayerPrefs已保存 - 窗户物品: {itemsStr}");

        // 保存当前场景名
        PlayerPrefs.SetString("CurrentScene", SceneManager.GetActiveScene().name);
    }

    private void RestoreVisualState()
    {
        Debug.Log($"恢复视觉状态: {currentState}");
        SpriteRenderer windowRenderer = GetComponent<SpriteRenderer>();

        switch (currentState)
        {
            case WindowState.Normal:
                // 默认状态
                shadowObject.SetActive(false);
                if (windowRenderer != null && originalWindowSprite != null)
                {
                    windowRenderer.sprite = originalWindowSprite;
                }
                break;
            case WindowState.PuzzleCompleted:
                // 已完成拼图，隐藏物品
                for (int i = 0; i < placedItems.Length; i++)
                {
                    if (placedItems[i] != null)
                        placedItems[i].gameObject.SetActive(false);
                }
                break;
            case WindowState.Black:
                // 窗户变黑
                if (windowRenderer != null && blackWindowSprite != null)
                {
                    windowRenderer.sprite = blackWindowSprite;
                }
                break;
            case WindowState.ShadowShown:
                // 显示人影
                shadowObject.SetActive(true);
                break;
            case WindowState.PostDialogue:
            case WindowState.Shadow1:
            case WindowState.Shadow2:
            case WindowState.Item:
                // 对话后的状态，恢复到PostDialogue等待点击
                currentState = WindowState.PostDialogue;
                if (windowRenderer != null && blackWindowSprite != null)
                {
                    windowRenderer.sprite = blackWindowSprite;
                }
                break;
        }
    }

    private void OnDialogueFinished()
    {
        currentState = WindowState.PostDialogue;
        Debug.Log("对话结束，进入后续剧情模式");
    }

    public override void CheckItem(ItemName itemName)
    {
        EmptyClicked();
    }

    public void CheckItemPlacement(Draggable item)
    {
        int index = item.targetIndex;
        float distance = Vector3.Distance(item.transform.position, targetPositions[index].position);
        Debug.Log($"检查物品放置: 物品{item.name}, 目标索引{index}, 距离{distance}");

        if (distance < 0.5f)
        {
            if (!item.isPlaced)
            {
                Debug.Log($"物品放置成功: 索引{index}");
                item.isPlaced = true;
                item.transform.position = targetPositions[index].position;
                SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = 10;
                placedItems[index] = item;
                completedCount++;
                CheckAllCompleted();
                SaveState(); // 保存物品放置状态
            }
        }
        else if (item.isPlaced)
        {
            Debug.Log($"物品移开: 索引{index}");
            item.isPlaced = false;
            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = 0;
            placedItems[index] = null;
            completedCount--;
            SaveState(); // 保存物品移除状态
        }
    }

    private void CheckAllCompleted()
    {
        if (completedCount == 4 && currentState == WindowState.Normal)
        {
            currentState = WindowState.PuzzleCompleted;
            Debug.Log("拼图完成，点击窗户继续");
        }
    }

    public override void EmptyClicked()
    {
        if (CursorManager.Instance.CurrentItem != ItemName.None)
        {
            SpawnItemInWindow(CursorManager.Instance.CurrentItem);
            EventHandler.CallItemUsedEvent(CursorManager.Instance.CurrentItem);
            SaveState(); // 保存状态
            return;
        }

        Debug.Log($"当前状态: {currentState}");
        switch (currentState)
        {
            case WindowState.PuzzleCompleted:
                for (int i = 0; i < placedItems.Length; i++)
                {
                    if (placedItems[i] != null)
                        placedItems[i].gameObject.SetActive(false);
                }
                SpriteRenderer windowSprite = GetComponent<SpriteRenderer>();
                windowSprite.sprite = blackWindowSprite;
                currentState = WindowState.Black;
                SaveState(); // 保存状态
                break;
            case WindowState.Black:
                shadowObject.SetActive(true);
                currentState = WindowState.ShadowShown;
                EventHandler.CallPuzzleCompletedEvent();
                SaveState(); // 保存状态
                break;
            case WindowState.ShadowShown:
                // 等待对话结束
                break;
            case WindowState.PostDialogue:
                // 人影消失
                shadowObject.transform.position = new Vector3(1000, 1000, 0);
                shadowObject.SetActive(false); // 确保隐藏
                currentState = WindowState.Shadow1;
                SaveState(); // 保存状态
                break;
            case WindowState.Shadow1:
                // 显示第二个人影（位置偏移）
                shadowObject.SetActive(true);
                shadowObject.transform.position = shadowOriginalPosition + new Vector3(0.07f, 0.32f, 0);
                if (shadowInteractive.newShadow1Sprite != null)
                {
                    shadowObject.GetComponent<SpriteRenderer>().sprite = shadowInteractive.newShadow1Sprite;
                }
                currentState = WindowState.Shadow2;
                SaveState(); // 保存状态
                break;
            case WindowState.Shadow2:
                // 显示第三个人影（位置偏移）
                shadowObject.transform.position = shadowOriginalPosition + new Vector3(0.86f, 0.37f, 0);
                if (shadowInteractive.newShadow2Sprite != null)
                {
                    shadowObject.GetComponent<SpriteRenderer>().sprite = shadowInteractive.newShadow2Sprite;
                }
                currentState = WindowState.Item;
                SaveState(); // 保存状态
                break;
            case WindowState.Item:
                // 生成物品到窗户旁边
                Vector3 itemPos = transform.position + new Vector3(0, -2f, 0);
                if (shadowInteractive.pickupItemPrefab != null)
                {
                    Instantiate(shadowInteractive.pickupItemPrefab, itemPos, Quaternion.identity);
                }
                shadowObject.SetActive(false);
                currentState = WindowState.Normal;
                SaveState(); // 保存状态
                break;
        }
    }

    private void SpawnItemInWindow(ItemName itemName)
    {
        GameObject itemPrefab = ItemPrefabDict.Instance.GetPrefab(itemName);
        if (itemPrefab == null) return;

        Vector2 spawnPos = GetRandomWindowPosition();
        GameObject item = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        Draggable draggable = item.GetComponent<Draggable>();
        if (draggable != null)
        {
            draggable.targetIndex = GetTargetIndexByItem(itemName);
            // 添加到窗户物品列表
            windowItems.Add(draggable);
            SaveState(); // 保存生成的新物品
        }
    }

    private Vector2 GetRandomWindowPosition()
    {
        Collider2D windowArea = GameObject.Find("WindowArea").GetComponent<Collider2D>();
        Bounds bounds = windowArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    private int GetTargetIndexByItem(ItemName itemName)
    {
        return itemData.GetItemDetails(itemName).targetPositionIndex;
    }

    // 添加辅助方法
    private ItemName GetItemNameByTargetIndex(int index)
    {
        // 根据targetPositionIndex反查物品名称
        foreach (var itemDetails in itemData.itemDetailsList)
        {
            if (itemDetails.targetPositionIndex == index)
            {
                return itemDetails.itemName;
            }
        }
        return ItemName.None;
    }
}
