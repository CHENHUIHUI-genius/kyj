//// 物品栏左右键
//using UnityEngine;
//using UnityEngine.UI;

//public class InventoryUI : MonoBehaviour
//{
//    public Button leftButton, rightButton;

//    public SlotUI slotUI;

//    public int currentIndex;        //显示UI当前的物品序号

//    private void OnEnable()
//    {
//        EventHandler.UpdateUIEvent += OnUpdateUIEvent;
//    }

//    private void OnDisable()
//    {
//        EventHandler.UpdateUIEvent -= OnUpdateUIEvent;
//    }

//    // 在拾取物品的时候会update一次
//    private void OnUpdateUIEvent(ItemDetails itemDetails, int index)
//    {
//        if (itemDetails == null)
//        {
//            slotUI.SetEmpty();
//            currentIndex = -1;
//            leftButton.interactable = false;
//            rightButton.interactable = false;
//        }
//        else
//        {
//            currentIndex = index;
//            slotUI.SetItem(itemDetails);

//            //        // 非空的时候也要修改一下左右按钮的状况
//            //        if (index > 0)
//            //        {
//            //            leftButton.interactable = true;
//            //        }
//            //        if (index == -1)
//            //        {
//            //            leftButton.interactable = false;
//            //            rightButton.interactable = false;
//            //        }
//        }
//    }


//}
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public SlotUI[] slots;          // 5 个槽
    private int selectedIndex = -1;

    //private void Start()
    //{
    //    foreach (var slot in slots)
    //    {
    //        if (slot != null && slot.selectedBG != null)
    //            slot.selectedBG.gameObject.SetActive(false);
    //    }
    //}

    private void Start()
    {
        // 初始化所有槽位为空且未选中
        foreach (var slot in slots)
        {
            slot.SetEmpty();
            if (slot.selectedBG != null)
                slot.selectedBG.gameObject.SetActive(false);
            slot.isSelected = false;
        }
    }

    private void OnEnable()
    {
        EventHandler.UpdateUIEvent += OnUpdateUIEvent;
        EventHandler.SlotClickedEvent += OnSlotClickedEvent;
    }

    private void OnDisable()
    {
        EventHandler.UpdateUIEvent -= OnUpdateUIEvent;
        EventHandler.SlotClickedEvent -= OnSlotClickedEvent;
    }

    // UpdateUIEvent: 物品更新时（拾取）                                                                 old
    //private void OnUpdateUIEvent(ItemDetails item, int index)
    //{
    //    if (item == null)
    //    {
    //        // 清空对应的槽
    //        slots[index].SetEmpty();
    //    }
    //    else
    //    {
    //        slots[index].SetItem(item);
    //    }
    //}

    private void OnUpdateUIEvent(ItemDetails item, int index)                                       //new
    {
        if (index < 0 || index >= slots.Length)
        {
            Debug.LogWarning($"[InventoryUI] Invalid slot index {index}, ignoring update.");
            return;
        }

        //if (item == null)
        //{
        //    slots[index].SetEmpty();
        //}
        if (item == null)
        {
            slots[index].SetEmpty();
            // 如果更新的槽位是当前选中的，取消选中
            if (selectedIndex == index)
            {
                slots[index].selectedBG.gameObject.SetActive(false);
                slots[index].isSelected = false;
                selectedIndex = -1;
                EventHandler.CallItemSelectedEvent(null, false);
            }
        }
        else
        {
            slots[index].SetItem(item);
        }
    }

    // 玩家点击某个槽
    private void OnSlotClickedEvent(int index)
    {
        //FindObjectOfType<SlotUI>()?.tooltip?.gameObject.SetActive(false);
        var slot = Object.FindFirstObjectByType<SlotUI>();
        slot?.tooltip?.gameObject.SetActive(false);
        //// 校验索引有效性
        //if (index < 0 || index >= slots.Length)
        //    return;

        //// 取消上一个选中槽位的状态
        //if (selectedIndex != -1)
        //{
        //    slots[selectedIndex].isSelected = false;
        //    slots[selectedIndex].selectedBG.gameObject.SetActive(false);
        //}

        //// 设置新选中的槽位
        //selectedIndex = index;
        //slots[index].isSelected = true;
        //slots[index].selectedBG.gameObject.SetActive(true);
        // 如果点到已选中槽 → 取消选中
        if (index < 0 || index >= slots.Length)
            return;
        if (selectedIndex == index)
        {
            slots[index].selectedBG.gameObject.SetActive(false);
            slots[index].isSelected = false;
            selectedIndex = -1;
            // 通知取消选中物品
            EventHandler.CallItemSelectedEvent(null, false);
            return;
        }

        // 如果之前有人被选中 → 取消它
        if (selectedIndex != -1)
        {
            slots[selectedIndex].selectedBG.gameObject.SetActive(false);
            slots[selectedIndex].isSelected = false; // 同步状态
            // 通知取消选中之前的物品
            EventHandler.CallItemSelectedEvent(null, false);
        }

        //// 新选中
        //selectedIndex = index;
        //slots[index].selectedBG.gameObject.SetActive(true);
        //slots[index].isSelected = true; // 同步状态
        //// 通知选中当前物品
        //EventHandler.CallItemSelectedEvent(slots[index].currentItem, true);
        // 选中新槽位（如果槽位有物品）
        if (slots[index].currentItem != null)
        {
            selectedIndex = index;
            slots[index].selectedBG.gameObject.SetActive(true);
            slots[index].isSelected = true;
            EventHandler.CallItemSelectedEvent(slots[index].currentItem, true);
        }
    }
}
