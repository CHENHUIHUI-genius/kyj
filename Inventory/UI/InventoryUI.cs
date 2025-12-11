using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public SlotUI[] slots;          // 5 个槽
    private int selectedIndex = -1;

    // 物品预制体字典（在Inspector赋值：Key=物品名，Value=对应预制体）
    public Dictionary<ItemName, GameObject> itemPrefabDict;


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
