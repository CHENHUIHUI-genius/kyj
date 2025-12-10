// 物品栏左右键
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;


//public class SlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler              // old
public class SlotUI : MonoBehaviour, IPointerClickHandler                                                           // new
{
    public Image selectedBG;   // back/highlight UI 图片              new
    public int slotIndex;      // 这是属于第几个槽（0~4）                new
    private Coroutine tooltipCoroutine;                                 // new
    public Image ItemImage;

    public ItemTooltip tooltip;

    public ItemDetails currentItem;                                                  // old
    //private bool isSelected;                                                          // old

    public bool isSelected = false;

    public void SetItem(ItemDetails itemDetails)
    {
        currentItem = itemDetails;
        this.gameObject.SetActive(true);                                         
        ItemImage.sprite = itemDetails.itemSprite;
        ItemImage.SetNativeSize();
        //selectedBG.gameObject.SetActive(isSelected); // 保持选中状态
    }


    public void SetEmpty()
    {
        currentItem = null;
        this.gameObject.SetActive(false);                                               // old
        selectedBG.gameObject.SetActive(false);                                           // new
        isSelected = false;
    }

    // 约定 “这个 UI 能处理鼠标 / 触摸点击事件”，必须写 OnPointerClick 方法（Unity 会在点击时自动调用）         old
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    isSelected = !isSelected;
    //    EventHandler.CallItemSelectedEvent(currentItem, isSelected);
    //}
    //                                                                                                  new
    public void OnPointerClick(PointerEventData eventData)
    {
        EventHandler.CallSlotClickedEvent(slotIndex);
        //// 切换选中状态
        //isSelected = !isSelected;
        //selectedBG.gameObject.SetActive(isSelected);

        //// 通知管理器选中状态（如果需要在其他地方处理选中逻辑）
        //if (isSelected)
        //{
        //    EventHandler.CallItemSelectedEvent(currentItem, true);
        //}
        //else
        //{
        //    EventHandler.CallItemSelectedEvent(null, false);
        //}
        // 显示 tooltip
        ShowTooltip();                                                                                     // new
    }

    //                                                                                                      以下new


    private void ShowTooltip()
    {
        if (currentItem == null) return;

        tooltip.UpdateItemName(currentItem.itemName);
        tooltip.gameObject.SetActive(true);

        // 如果之前有协程，先停止
        if (tooltipCoroutine != null)
            StopCoroutine(tooltipCoroutine);

        tooltipCoroutine = StartCoroutine(HideTooltipDelay());
    }

    private IEnumerator HideTooltipDelay()
    {
        yield return new WaitForSeconds(2f);   // 2 秒后隐藏，可自行修改
        tooltip.gameObject.SetActive(false);
    }



    //// 约定 “这个 UI 能处理鼠标移入事件”，必须写 OnPointerEnter 方法（鼠标移到 UI 上时自动调用）
    //public void OnPointerEnter(PointerEventData eventData)                                      // 改成点击时出现itemname，过几秒后消失
    //{
    //    if (this.gameObject.activeInHierarchy)
    //    {
    //        tooltip.gameObject.SetActive(true);
    //        tooltip.UpdateItemName(currentItem.itemName);
    //    }
    //}

    //// 约定 “这个 UI 能处理鼠标移出事件”，必须写 OnPointerExit 方法（鼠标离开 UI 时自动调用）
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    tooltip.gameObject.SetActive(false);
    //}
}
