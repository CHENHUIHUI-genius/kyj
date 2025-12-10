using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class CursorManager : MonoBehaviour
{
    public RectTransform back;                      // delete
    private Vector3 mouseWorldPos => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    // 鼠标位置转换成世界坐标

    private ItemName currentItem;

    private bool canClick;
    private bool holdItem;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.ItemUsedEvent += OnItemUsedEvent;
    }


    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.ItemUsedEvent -= OnItemUsedEvent;
    }

    private void OnItemUsedEvent(ItemName obj)
    {
        currentItem = ItemName.None;
        //holdItem = false;
        //hand.gameObject.SetActive(false);                                               // delete
    }


    private void Update()
    {
        // 1. 实时更新"是否可点击"状态：通过检测鼠标位置是否存在目标对象（由ObjectAtMousePosition()判断）
        canClick = ObjectAtMousePosition();

        //// 2. 鼠标指针（hand）位置同步逻辑 条件：如果鼠标指针对象（hand）处于激活状态（在Hierarchy中显示）
        ////if (hand.gameObject.activeInHierarchy)                                         // delete
        ////    hand.position = Input.mousePosition;                                       // delete    用于实现手和鼠标跟随，本设计为蒙版不需要

        //// 3. UI交互检测：如果当前鼠标正在与UI交互（如点击按钮、输入框）
        //// 直接返回，终止后续所有点击逻辑（避免UI交互与场景物体点击冲突）
        //if (InteractWithUI())
        //    return;


        // 4. 合法点击判断与执行：满足两个条件才触发点击行为
        // 条件1：canClick为true → 鼠标位置有可点击的场景对象
        // 条件2：Input.GetMouseButtonDown(0) → 玩家按下鼠标左键（0=左键，1=右键，2=中键）
        if (canClick && Input.GetMouseButtonDown(0))
        {
            // 执行点击动作：将鼠标位置的可点击对象（GameObject）传入点击处理方法
            // ClickAction()：自定义点击逻辑方法（推测功能：根据点击的对象执行对应操作，如"teleport"，"interactive"等）
            ClickAction(ObjectAtMousePosition().gameObject);
        }
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //holdItem = isSelected;
        if (isSelected)
        {
            currentItem = itemDetails.itemName;
        }
        //back.gameObject.SetActive(holdItem);                      // delete
    }


    private void ClickAction(GameObject clickObject)
    {
        switch (clickObject.tag)
        {
            case "Teleport":
                var teleport = clickObject.GetComponent<Teleport>();
                teleport?.TeleportToScene();
                break;
            case "Item":
                var item = clickObject.GetComponent<Item>();
                item?.ItemClicked();
                break;
            // 根据玩家当前是否 “持有物品”，调用互动对象（如门、机关）的对应方法，把 “玩家操作” 和 “互动逻辑” 串联起来
            case "Interactive":
                var interactive = clickObject.GetComponent<Interactive>();
                if (currentItem != ItemName.None)
                    interactive?.CheckItem(currentItem);
                else
                    interactive?.EmptyClicked();
                break;
        }
    }

    private Collider2D ObjectAtMousePosition()
    {
        return Physics2D.OverlapPoint(mouseWorldPos);
    }

    ///// 检测当前鼠标/触摸是否正与UI元素交互的工具方法
    ///// 核心作用：判断输入事件（如点击、拖拽）是否落在UI上，避免UI交互与场景物体交互冲突（比如点击按钮时同时触发场景物体的点击）
    //private bool InteractWithUI()
    //{
    //    // 条件1：EventSystem.current != null → 确保场景中存在EventSystem（UI交互的核心管理器）
    //    // 场景中没有EventSystem时，UI无法交互，此时直接返回false（避免空引用异常）
    //    // 条件2：EventSystem.current.IsPointerOverGameObject() → 检测当前鼠标指针（或触摸点）是否悬浮/点击在UI元素上
    //    // IsPointerOverGameObject()：EventSystem内置方法，返回true表示指针在UI上（如按钮、图片、输入框等带Collider/GraphicRaycaster的UI）
    //    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
    //        return true;
    //    else
    //        return false;
    //}

}
