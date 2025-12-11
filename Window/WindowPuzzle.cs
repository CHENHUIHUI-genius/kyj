// WindowPuzzle.cs
using UnityEngine;

public class WindowPuzzle : Interactive
{
    public Transform[] targetPositions; // 4个目标位置（在Inspector赋值）
    private Draggable[] placedItems = new Draggable[4]; // 已正确放置的物品
    private int completedCount = 0; // 已完成的拼图数量

    public ItemDataList_SO itemData;
    public Sprite blackWindowSprite; // 在Inspector中赋值黑色窗户图片
    private enum WindowState { Normal, PuzzleCompleted, Black, ShadowShown }
    private WindowState currentState = WindowState.Normal;

    private void Start()
    {
        // 初始化目标位置（与物品的targetIndex对应）
    }

    public override void CheckItem(ItemName itemName)
    {
        // 对于窗户谜题，总是执行空点击逻辑（生成物品）
        EmptyClicked();
    }

    // 检测物品是否放在正确位置
    //public void CheckItemPlacement(Draggable item)
    //{
    //    int index = item.targetIndex;
    //    // 检查物品是否接近目标位置（允许一定误差）
    //    if (Vector2.Distance(item.transform.position, targetPositions[index].position) < 50f)
    //    {
    //        if (!item.isPlaced)
    //        {
    //            item.isPlaced = true;
    //            item.transform.position = targetPositions[index].position; // 吸附到目标位置
    //            placedItems[index] = item;
    //            completedCount++;
    //            CheckAllCompleted();
    //        }
    //    }
    //    else if (item.isPlaced)
    //    {
    //        // 已放置但被移开
    //        item.isPlaced = false;
    //        placedItems[index] = null;
    //        completedCount--;
    //    }
    //}
    //public void CheckItemPlacement(Draggable item)
    //{
    //    int index = item.targetIndex;

    //    // 检查物品是否接近目标位置（使用世界距离）
    //    if (Vector3.Distance(item.transform.position, targetPositions[index].position) < 0.5f) // 调整距离阈值
    //    {
    //        if (!item.isPlaced)
    //        {
    //            item.isPlaced = true;
    //            item.transform.position = targetPositions[index].position; // 吸附到世界位置
    //            placedItems[index] = item;
    //            completedCount++;
    //            CheckAllCompleted();
    //        }
    //    }
    //    else if (item.isPlaced)
    //    {
    //        item.isPlaced = false;
    //        placedItems[index] = null;
    //        completedCount--;
    //    }
    //}
    public void CheckItemPlacement(Draggable item)
    {
        int index = item.targetIndex;
        if (Vector3.Distance(item.transform.position, targetPositions[index].position) < 0.5f)
        {
            if (!item.isPlaced)
            {
                item.isPlaced = true;
                item.transform.position = targetPositions[index].position;
                // 已放置的物品显示在前面
                SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = 0;
                }
                placedItems[index] = item;
                completedCount++;
                CheckAllCompleted();
            }
        }
        else if (item.isPlaced)
        {
            item.isPlaced = false;
            // 取消放置时恢复正常顺序
            SpriteRenderer sr = item.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 10;
            }
            placedItems[index] = null;
            completedCount--;
        }
    }


    // 检查所有拼图是否完成
    //private void CheckAllCompleted()
    //{
    //    if (completedCount == 4)
    //    {
    //        // 触发窗户变黑和出现人影
    //        OnPuzzleCompleted();
    //    }
    //}

    private void CheckAllCompleted()
    {
        if (completedCount == 4 && currentState == WindowState.Normal)
        {
            currentState = WindowState.PuzzleCompleted;
            // 可以添加闪烁效果或提示，表示可以点击
            Debug.Log("拼图完成，点击窗户继续");
        }
    }

    private void OnPuzzleCompleted()
    {
        // 1. 窗户变黑（修改窗户Sprite或添加黑色遮罩）
        SpriteRenderer windowSprite = GetComponent<SpriteRenderer>();
        //windowSprite.color = Color.black;
        windowSprite.sprite = blackWindowSprite;

        // 2. 显示人影（激活人影对象）
        GameObject shadow = transform.Find("Shadow").gameObject;
        shadow.SetActive(true);

        // 3. 触发后续事件（如剧情）
        EventHandler.CallPuzzleCompletedEvent();
    }

    // 重写空点击方法（点击窗户时如果有选中物品，生成物品到窗户区域）
    //public override void EmptyClicked()
    //{
    //    if (CursorManager.Instance.CurrentItem != ItemName.None)
    //    {
    //        // 生成选中的物品到窗户区域
    //        SpawnItemInWindow(CursorManager.Instance.CurrentItem);
    //        // 从物品栏移除该物品
    //        EventHandler.CallItemUsedEvent(CursorManager.Instance.CurrentItem);
    //    }
    //}

    public override void EmptyClicked()
    {
        if (CursorManager.Instance.CurrentItem != ItemName.None)
        {
            SpawnItemInWindow(CursorManager.Instance.CurrentItem);
            EventHandler.CallItemUsedEvent(CursorManager.Instance.CurrentItem);
            return;
        }

        // 根据状态处理点击
        switch (currentState)
        {
            case WindowState.PuzzleCompleted:
                // 隐藏所有已放置的物品
                for (int i = 0; i < placedItems.Length; i++)
                {
                    if (placedItems[i] != null)
                    {
                        placedItems[i].gameObject.SetActive(false);
                    }
                }
                // 变黑
                SpriteRenderer windowSprite = GetComponent<SpriteRenderer>();
                windowSprite.sprite = blackWindowSprite;
                //windowSprite.color = Color.black;
                currentState = WindowState.Black;
                break;
            case WindowState.Black:
                // 显示人影
                GameObject shadow = transform.Find("Shadow").gameObject;
                shadow.SetActive(true);
                currentState = WindowState.ShadowShown;
                EventHandler.CallPuzzleCompletedEvent();
                break;
            case WindowState.ShadowShown:
                // 可选：再次点击隐藏人影或触发其他事件
                break;
        }
    }


    // 在窗户区域随机位置生成物品
    private void SpawnItemInWindow(ItemName itemName)
    {
        // 根据物品名称获取对应的prefab（需要提前在字典中注册）
        GameObject itemPrefab = ItemPrefabDict.Instance.GetPrefab(itemName);
        GameObject item = Instantiate(itemPrefab, GetRandomWindowPosition(), Quaternion.identity);
        // 设置物品的目标索引（根据物品类型确定）
        item.GetComponent<Draggable>().targetIndex = GetTargetIndexByItem(itemName);
    }
    //private void SpawnItemInWindow(ItemName itemName)
    //{
    //    Debug.Log($"尝试生成物品: {itemName}");
    //    GameObject itemPrefab = ItemPrefabDict.Instance.GetPrefab(itemName);
    //    if (itemPrefab == null)
    //    {
    //        Debug.LogError($"未找到物品 {itemName} 的预制体");
    //        return;
    //    }

    //    Vector2 spawnPos = GetRandomWindowPosition();
    //    Debug.Log($"生成位置: {spawnPos}");
    //    GameObject item = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
    //    item.GetComponent<Draggable>().targetIndex = GetTargetIndexByItem(itemName);
    //    Debug.Log($"物品已生成: {item.name}");
    //}

    // 随机生成窗户区域内的位置
    //private Vector2 GetRandomWindowPosition()
    //{
    //    RectTransform windowArea = GameObject.Find("WindowArea").GetComponent<RectTransform>();
    //    float x = Random.Range(windowArea.rect.xMin, windowArea.rect.xMax);
    //    float y = Random.Range(windowArea.rect.yMin, windowArea.rect.yMax);
    //    return windowArea.TransformPoint(new Vector2(x, y));
    //}

    private Vector2 GetRandomWindowPosition()
    {
        Collider2D windowArea = GameObject.Find("WindowArea").GetComponent<Collider2D>();
        Bounds bounds = windowArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(x, y);
    }

    // 根据物品类型获取目标位置索引（需自定义映射关系）
    //private int GetTargetIndexByItem(ItemName itemName)
    //{
    //    switch (itemName)
    //    {
    //        case ItemName.paper1: return 0;
    //        case ItemName.paper2: return 1;
    //        case ItemName.paper3: return 2;
    //        case ItemName.paper4: return 3;
    //        default: return -1;
    //    }
    //}

    private int GetTargetIndexByItem(ItemName itemName)
    {
        // 从ItemData中获取物品详情，直接返回配置的targetPositionIndex
        return itemData.GetItemDetails(itemName).targetPositionIndex;
    }
}