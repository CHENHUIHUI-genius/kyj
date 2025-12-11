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
            // ������ҵ�ǰ�Ƿ� ��������Ʒ�������û����������š����أ��Ķ�Ӧ�������� ����Ҳ����� �� �������߼��� ��������
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
