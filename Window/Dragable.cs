//// Draggable.cs
//using UnityEngine;
//using UnityEngine.EventSystems;

//public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
//{
//    private RectTransform rectTransform;
//    private Canvas canvas;
//    private Vector2 originalPosition; // 初始位置（物品栏位置）
//    [HideInInspector] public bool isPlaced = false; // 是否已放置在正确位置
//    [HideInInspector] public int targetIndex; // 对应的目标位置索引

//    private void Awake()
//    {
//        rectTransform = GetComponent<RectTransform>();
//        canvas = FindFirstObjectByType<Canvas>();
//        originalPosition = rectTransform.anchoredPosition;
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        if (isPlaced) return; // 已正确放置的物品不能再拖动
//        originalPosition = rectTransform.anchoredPosition; // 记录拖拽前位置
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        if (isPlaced) return;
//        // 跟随鼠标移动（适配Canvas缩放）
//        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        if (isPlaced) return;
//        // 检查是否拖到窗户区域内
//        if (IsInWindowArea())
//        {
//            // 保持当前位置（在窗户内）
//            // 通知窗户检测是否放在正确位置
//            FindFirstObjectByType<WindowPuzzle>().CheckItemPlacement(this);
//        }
//        else
//        {
//            // 未在窗户区域，返回物品栏
//            rectTransform.anchoredPosition = originalPosition;
//        }
//    }

//    // 检查是否在窗户可放置区域内
//    private bool IsInWindowArea()
//    {
//        Collider2D windowArea = GameObject.Find("WindowArea").GetComponent<Collider2D>();
//        return windowArea.OverlapPoint(rectTransform.position);
//    }
//}

//using UnityEngine;
//using UnityEngine.EventSystems;

//public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
//{
//    private Transform itemTransform; // 改为Transform
//    private Vector3 originalPosition; // 改为Vector3
//    [HideInInspector] public bool isPlaced = false;
//    [HideInInspector] public int targetIndex;

//    private void Awake()
//    {
//        itemTransform = GetComponent<Transform>(); // 获取Transform
//        originalPosition = itemTransform.position; // 记录世界位置
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        if (isPlaced) return;
//        originalPosition = itemTransform.position; // 记录拖拽前世界位置
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        if (isPlaced) return;
//        // 世界空间拖拽：直接设置世界位置
//        itemTransform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        itemTransform.position = new Vector3(itemTransform.position.x, itemTransform.position.y, 0); // 保持z=0
//    }

//    public void OnEndDrag(PointerEventData eventData)
//    {
//        if (isPlaced) return;
//        // 检查是否在窗户区域内
//        if (IsInWindowArea())
//        {
//            // 保持当前位置
//            FindFirstObjectByType<WindowPuzzle>().CheckItemPlacement(this);
//        }
//        else
//        {
//            // 返回原位置
//            itemTransform.position = originalPosition;
//        }
//    }

//    private bool IsInWindowArea()
//    {
//        Collider2D windowArea = GameObject.Find("WindowArea").GetComponent<Collider2D>();
//        return windowArea.OverlapPoint(itemTransform.position); // 使用世界位置
//    }
//}
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Transform itemTransform;
    private Vector3 originalPosition;
    private bool isDragging = false;
    [HideInInspector] public bool isPlaced = false;
    [HideInInspector] public int targetIndex;

    private void Awake()
    {
        itemTransform = GetComponent<Transform>();
        originalPosition = itemTransform.position;
    }

    private void Update()
    {
        if (isPlaced) return;

        // 检测鼠标按下
        if (Input.GetMouseButtonDown(0))
        {
            // 检查鼠标是否在物品上
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.gameObject == gameObject)
            {
                isDragging = true;
                originalPosition = itemTransform.position;
            }
        }

        // 拖拽中
        if (isDragging && Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            itemTransform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0);
        }

        // 鼠标释放
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            // 检查是否在窗户区域内
            if (IsInWindowArea())
            {
                FindFirstObjectByType<WindowPuzzle>().CheckItemPlacement(this);
            }
            else
            {
                itemTransform.position = originalPosition;
            }
        }
    }

    private bool IsInWindowArea()
    {
        Collider2D windowArea = GameObject.Find("WindowArea").GetComponent<Collider2D>();
        return windowArea.OverlapPoint(itemTransform.position);
    }
}
