using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public RectTransform back;                      // delete
    private Vector3 mouseWorldPos => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    // ���λ��ת������������

    private ItemName currentItem;

    private bool canClick;
    private bool holdItem;

    public ItemName CurrentItem => currentItem; // ����
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
        // 1. ʵʱ����"�Ƿ�ɵ��"״̬��ͨ��������λ���Ƿ����Ŀ�������ObjectAtMousePosition()�жϣ�
        canClick = ObjectAtMousePosition();

        //// 2. ���ָ�루hand��λ��ͬ���߼� ������������ָ�����hand�����ڼ���״̬����Hierarchy����ʾ��
        ////if (hand.gameObject.activeInHierarchy)                                         // delete
        ////    hand.position = Input.mousePosition;                                       // delete    ����ʵ���ֺ������棬�����Ϊ�ɰ治��Ҫ

        //// 3. UI������⣺�����ǰ���������UI������������ť�������
        //// ֱ�ӷ��أ���ֹ�������е���߼�������UI�����볡����������ͻ��
        //if (InteractWithUI())
        //    return;


        // 4. �Ϸ�����ж���ִ�У��������������Ŵ��������Ϊ
        // ����1��canClickΪtrue �� ���λ���пɵ���ĳ�������
        // ����2��Input.GetMouseButtonDown(0) �� ��Ұ�����������0=�����1=�Ҽ���2=�м���
        if (canClick && Input.GetMouseButtonDown(0))
        {
            // ִ�е�������������λ�õĿɵ������GameObject����������������
            // ClickAction()���Զ������߼��������Ʋ⹦�ܣ����ݵ���Ķ���ִ�ж�Ӧ��������"teleport"��"interactive"�ȣ�
            ClickAction(ObjectAtMousePosition().gameObject);
        }
    }
    //private void Update()
    //{
    //    Collider2D hit = ObjectAtMousePosition();
    //    canClick = hit != null;

    //    if (canClick && hit.gameObject.CompareTag("Interactive"))
    //    {
    //        Debug.Log($"检测到Interactive对象: {hit.gameObject.name}");
    //    }

    //    if (canClick && Input.GetMouseButtonDown(0))
    //    {
    //        GameObject clickedObj = hit.gameObject;
    //        Debug.Log($"点击Interactive对象: {clickedObj.name}, 脚本: {clickedObj.GetComponent<Interactive>()}");
    //        ClickAction(clickedObj);
    //    }
    //}

    //private void Update()
    //{
    //    Collider2D hit = ObjectAtMousePosition();
    //    canClick = hit != null;

    //    if (canClick)
    //    {
    //        Debug.Log($"鼠标位置检测到: {hit.gameObject.name}, Tag: {hit.gameObject.tag}");
    //    }

    //    if (canClick && Input.GetMouseButtonDown(0))
    //    {
    //        GameObject clickedObj = hit.gameObject;
    //        Debug.Log($"点击对象: {clickedObj.name}");
    //        ClickAction(clickedObj);
    //    }
    //}

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
            // ƷûšأĶӦ Ҳ  ߼ 
            case "Interactive":
                var interactive = clickObject.GetComponent<Interactive>();
                Debug.Log($"Interactive组件: {interactive}, 类型: {interactive?.GetType()}");
                if (currentItem != ItemName.None)
                    interactive?.CheckItem(currentItem);
                else
                    interactive?.EmptyClicked();
                break;

        }
    }

    //private Collider2D ObjectAtMousePosition()
    //{
    //    return Physics2D.OverlapPoint(mouseWorldPos);
    //}
    //private Collider2D ObjectAtMousePosition()
    //{
    //    Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

    //    // 获取鼠标位置的所有碰撞体
    //    Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos);

    //    // 优先返回有"Interactive"标签的对象
    //    foreach (Collider2D hit in hits)
    //    {
    //        if (hit.gameObject.CompareTag("Interactive"))
    //        {
    //            return hit;
    //        }
    //    }

    //    // 如果没有Interactive对象，返回第一个碰撞体（如果有的话）
    //    return hits.Length > 0 ? hits[0] : null;
    //}

    private Collider2D ObjectAtMousePosition()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        // 获取鼠标位置的所有碰撞体
        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos);

        // 优先返回名为"Shadow"的对象
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.name == "Shadow")
            {
                return hit;
            }
        }

        // 然后返回其他Interactive对象
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag("Interactive"))
            {
                return hit;
            }
        }

        // 最后返回第一个碰撞体
        return hits.Length > 0 ? hits[0] : null;
    }


    ///// ��⵱ǰ���/�����Ƿ�����UIԪ�ؽ����Ĺ��߷���
    ///// �������ã��ж������¼�����������ק���Ƿ�����UI�ϣ�����UI�����볡�����彻����ͻ����������ťʱͬʱ������������ĵ����
    //private bool InteractWithUI()
    //{
    //    // ����1��EventSystem.current != null �� ȷ�������д���EventSystem��UI�����ĺ��Ĺ�������
    //    // ������û��EventSystemʱ��UI�޷���������ʱֱ�ӷ���false������������쳣��
    //    // ����2��EventSystem.current.IsPointerOverGameObject() �� ��⵱ǰ���ָ�루�����㣩�Ƿ�����/�����UIԪ����
    //    // IsPointerOverGameObject()��EventSystem���÷���������true��ʾָ����UI�ϣ��簴ť��ͼƬ�������ȴ�Collider/GraphicRaycaster��UI��
    //    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
    //        return true;
    //    else
    //        return false;
    //}

}
