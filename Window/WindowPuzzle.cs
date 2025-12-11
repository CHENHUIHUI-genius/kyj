// WindowPuzzle.cs
using UnityEngine;

public class WindowPuzzle : Interactive
{
    public Transform[] targetPositions; // 4个目标位置（在Inspector赋值）
    private Draggable[] placedItems = new Draggable[4]; // 已正确放置的物品
    private int completedCount = 0; // 已完成的拼图数量

    public ItemDataList_SO itemData;
    private void Start()
    {
        // 初始化目标位置（与物品的targetIndex对应）
    }

    // 检测物品是否放在正确位置
    public void CheckItemPlacement(Draggable item)
    {
        int index = item.targetIndex;
        // 检查物品是否接近目标位置（允许一定误差）
        if (Vector2.Distance(item.transform.position, targetPositions[index].position) < 50f)
        {
            if (!item.isPlaced)
            {
                item.isPlaced = true;
                item.transform.position = targetPositions[index].position; // 吸附到目标位置
                placedItems[index] = item;
                completedCount++;
                CheckAllCompleted();
            }
        }
        else if (item.isPlaced)
        {
            // 已放置但被移开
            item.isPlaced = false;
            placedItems[index] = null;
            completedCount--;
        }
    }

    // 检查所有拼图是否完成
    private void CheckAllCompleted()
    {
        if (completedCount == 4)
        {
            // 触发窗户变黑和出现人影
            OnPuzzleCompleted();
        }
    }

    private void OnPuzzleCompleted()
    {
        // 1. 窗户变黑（修改窗户Sprite或添加黑色遮罩）
        SpriteRenderer windowSprite = GetComponent<SpriteRenderer>();
        windowSprite.color = Color.black;

        // 2. 显示人影（激活人影对象）
        GameObject shadow = transform.Find("Shadow").gameObject;
        shadow.SetActive(true);

        // 3. 触发后续事件（如剧情）
        EventHandler.CallPuzzleCompletedEvent();
    }

    // 重写空点击方法（点击窗户时如果有选中物品，生成物品到窗户区域）
    public override void EmptyClicked()
    {
        if (CursorManager.Instance.CurrentItem != ItemName.None)
        {
            // 生成选中的物品到窗户区域
            SpawnItemInWindow(CursorManager.Instance.CurrentItem);
            // 从物品栏移除该物品
            EventHandler.CallItemUsedEvent(CursorManager.Instance.CurrentItem);
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

    // 随机生成窗户区域内的位置
    private Vector2 GetRandomWindowPosition()
    {
        RectTransform windowArea = GameObject.Find("WindowArea").GetComponent<RectTransform>();
        float x = Random.Range(windowArea.rect.xMin, windowArea.rect.xMax);
        float y = Random.Range(windowArea.rect.yMin, windowArea.rect.yMax);
        return windowArea.TransformPoint(new Vector2(x, y));
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