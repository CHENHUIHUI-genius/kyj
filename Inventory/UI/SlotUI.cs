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


    public void OnPointerClick(PointerEventData eventData)
    {
        EventHandler.CallSlotClickedEvent(slotIndex);

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


}
