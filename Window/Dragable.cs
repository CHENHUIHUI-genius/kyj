// Draggable.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalPosition; // 初始位置（物品栏位置）
    [HideInInspector] public bool isPlaced = false; // 是否已放置在正确位置
    [HideInInspector] public int targetIndex; // 对应的目标位置索引

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = FindFirstObjectByType<Canvas>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced) return; // 已正确放置的物品不能再拖动
        originalPosition = rectTransform.anchoredPosition; // 记录拖拽前位置
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        // 跟随鼠标移动（适配Canvas缩放）
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced) return;
        // 检查是否拖到窗户区域内
        if (IsInWindowArea())
        {
            // 保持当前位置（在窗户内）
            // 通知窗户检测是否放在正确位置
            FindFirstObjectByType<WindowPuzzle>().CheckItemPlacement(this);
        }
        else
        {
            // 未在窗户区域，返回物品栏
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    // 检查是否在窗户可放置区域内
    private bool IsInWindowArea()
    {
        Collider2D windowArea = GameObject.Find("WindowArea").GetComponent<Collider2D>();
        return windowArea.OverlapPoint(rectTransform.position);
    }
}