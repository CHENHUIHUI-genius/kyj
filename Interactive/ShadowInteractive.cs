//////using UnityEngine;

//////public class ShadowInteractive : Interactive
//////{
//////    public DialogueData_SO shadowDialogue; // ÔÚInspector¸³Öµ

//////    // 重写CheckItem，总是触发对话（不管有没有选中物品）
//////    public override void CheckItem(ItemName itemName)
//////    {
//////        if (shadowDialogue == null)
//////        {
//////            Debug.LogError("shadowDialogue没有赋值");
//////            return;
//////        }
//////        DialogueManager.Instance.StartDialogue(shadowDialogue);
//////    }

//////    // 重写EmptyClicked，也触发对话
//////    public override void EmptyClicked()
//////    {
//////        DialogueManager.Instance.StartDialogue(shadowDialogue);
//////    }
//////}

////using UnityEngine;

////public class ShadowInteractive : Interactive
////{
////    public DialogueData_SO shadowDialogue;
////    public Sprite newShadow1Sprite; // 第二个人影图片
////    public Sprite newShadow2Sprite; // 第三个人影图片
////    public GameObject pickupItemPrefab; // 可捡物品预制体

////    private enum PostDialogueState { Waiting, Shadow1, Shadow2, Item }
////    private PostDialogueState currentState = PostDialogueState.Waiting;
////    private SpriteRenderer shadowRenderer;

////    private void Start()
////    {
////        shadowRenderer = GetComponent<SpriteRenderer>();
////    }

////    private void OnEnable()
////    {
////        EventHandler.DialogueFinishedEvent += OnDialogueFinished;
////    }

////    private void OnDisable()
////    {
////        EventHandler.DialogueFinishedEvent -= OnDialogueFinished;
////    }

////    private void OnDialogueFinished()
////    {
////        currentState = PostDialogueState.Waiting;
////        Debug.Log("对话结束，等待点击继续剧情");
////    }

////    // 重写点击方法
////    public override void CheckItem(ItemName itemName)
////    {
////        HandlePostDialogueClick();
////    }

////    public override void EmptyClicked()
////    {
////        HandlePostDialogueClick();
////    }

////    private void HandlePostDialogueClick()
////    {
////        switch (currentState)
////        {
////            case PostDialogueState.Waiting:
////                // 隐藏当前人影
////                gameObject.SetActive(false);
////                currentState = PostDialogueState.Shadow1;
////                break;
////            case PostDialogueState.Shadow1:
////                // 显示第二个人影
////                gameObject.SetActive(true);
////                shadowRenderer.sprite = newShadow1Sprite;
////                currentState = PostDialogueState.Shadow2;
////                break;
////            case PostDialogueState.Shadow2:
////                // 显示第三个人影
////                shadowRenderer.sprite = newShadow2Sprite;
////                currentState = PostDialogueState.Item;
////                break;
////            case PostDialogueState.Item:
////                // 生成可捡物品
////                Instantiate(pickupItemPrefab, transform.position, Quaternion.identity);
////                // 隐藏人影
////                gameObject.SetActive(false);
////                break;
////        }
////    }
////}

//using UnityEngine;

//public class ShadowInteractive : Interactive
//{
//    public DialogueData_SO shadowDialogue;
//    public Sprite newShadow1Sprite;
//    public Sprite newShadow2Sprite;
//    public GameObject pickupItemPrefab;

//    private enum PostDialogueState { None, Waiting, Shadow1, Shadow2, Item }
//    private PostDialogueState currentState = PostDialogueState.None;
//    private SpriteRenderer shadowRenderer;
//    //private bool dialogueStarted = false;
//    public bool dialogueStarted = false;
//    private Vector3 originalPosition;

//    private void Start()
//    {
//        shadowRenderer = GetComponent<SpriteRenderer>();
//        originalPosition = transform.position; // 保存原位置
//    }

//    private void OnEnable()
//    {
//        EventHandler.DialogueFinishedEvent += OnDialogueFinished;
//    }

//    private void OnDisable()
//    {
//        EventHandler.DialogueFinishedEvent -= OnDialogueFinished;
//    }

//    private void OnDialogueFinished()
//    {
//        dialogueStarted = true;
//        currentState = PostDialogueState.Waiting;
//        Debug.Log("对话结束，等待点击继续剧情");
//    }

//    // 重写点击方法
//    public override void CheckItem(ItemName itemName)
//    {
//        if (!dialogueStarted)
//        {
//            // 第一次点击，开始对话
//            DialogueManager.Instance.StartDialogue(shadowDialogue);
//        }
//        else
//        {
//            // 对话后点击，执行状态机
//            HandlePostDialogueClick();
//        }
//    }

//    public override void EmptyClicked()
//    {
//        if (!dialogueStarted)
//        {
//            // 第一次点击，开始对话
//            DialogueManager.Instance.StartDialogue(shadowDialogue);
//        }
//        else
//        {
//            // 对话后点击，执行状态机
//            HandlePostDialogueClick();
//        }
//    }

//    //private void HandlePostDialogueClick()
//    //{
//    //    switch (currentState)
//    //    {
//    //        case PostDialogueState.Waiting:
//    //            // 隐藏当前人影
//    //            gameObject.SetActive(false);
//    //            currentState = PostDialogueState.Shadow1;
//    //            break;
//    //        case PostDialogueState.Shadow1:
//    //            // 显示第二个人影
//    //            gameObject.SetActive(true);
//    //            shadowRenderer.sprite = newShadow1Sprite;
//    //            currentState = PostDialogueState.Shadow2;
//    //            break;
//    //        case PostDialogueState.Shadow2:
//    //            // 显示第三个人影
//    //            shadowRenderer.sprite = newShadow2Sprite;
//    //            currentState = PostDialogueState.Item;
//    //            break;
//    //        case PostDialogueState.Item:
//    //            // 生成可捡物品
//    //            Instantiate(pickupItemPrefab, transform.position, Quaternion.identity);
//    //            // 隐藏人影
//    //            gameObject.SetActive(false);
//    //            break;
//    //    }
//    //}
//    //private void HandlePostDialogueClick()
//    //{
//    //    Debug.Log($"当前状态: {currentState}");
//    //    switch (currentState)
//    //    {
//    //        case PostDialogueState.Waiting:
//    //            Debug.Log("隐藏人影");
//    //            gameObject.SetActive(false);
//    //            currentState = PostDialogueState.Shadow1;
//    //            break;
//    //        case PostDialogueState.Shadow1:
//    //            Debug.Log("显示第二个人影");
//    //            gameObject.SetActive(true);
//    //            if (newShadow1Sprite != null)
//    //            {
//    //                shadowRenderer.sprite = newShadow1Sprite;
//    //                Debug.Log("设置Sprite成功");
//    //            }
//    //            else
//    //            {
//    //                Debug.LogError("newShadow1Sprite没有赋值");
//    //            }
//    //            currentState = PostDialogueState.Shadow2;
//    //            break;
//    //        case PostDialogueState.Shadow2:
//    //            Debug.Log("显示第三个人影");
//    //            if (newShadow2Sprite != null)
//    //            {
//    //                shadowRenderer.sprite = newShadow2Sprite;
//    //            }
//    //            currentState = PostDialogueState.Item;
//    //            break;
//    //        case PostDialogueState.Item:
//    //            Debug.Log("生成物品");
//    //            if (pickupItemPrefab != null)
//    //            {
//    //                Instantiate(pickupItemPrefab, transform.position, Quaternion.identity);
//    //            }
//    //            gameObject.SetActive(false);
//    //            break;
//    //    }
//    //}

//    private void HandlePostDialogueClick()
//    {
//        Debug.Log($"当前状态: {currentState}");
//        switch (currentState)
//        {
//            case PostDialogueState.Waiting:
//                Debug.Log("隐藏人影");
//                // 移出屏幕而不是SetActive(false)
//                transform.position = new Vector3(1000, 1000, 0); // 移到屏幕外
//                currentState = PostDialogueState.Shadow1;
//                break;
//            case PostDialogueState.Shadow1:
//                Debug.Log("显示第二个人影");
//                // 移回原位置
//                transform.position = originalPosition; // 需要保存原位置
//                shadowRenderer.sprite = newShadow1Sprite;
//                currentState = PostDialogueState.Shadow2;
//                break;
//            case PostDialogueState.Shadow2:
//                Debug.Log("显示第三个人影");
//                shadowRenderer.sprite = newShadow2Sprite;
//                currentState = PostDialogueState.Item;
//                break;
//            case PostDialogueState.Item:
//                Debug.Log("生成物品");
//                Instantiate(pickupItemPrefab, transform.position, Quaternion.identity);
//                // 移出屏幕
//                transform.position = new Vector3(1000, 1000, 0);
//                break;
//        }
//    }
//    // 供Window调用的方法
//    public void HandlePostDialogueClickFromWindow()
//    {
//        HandlePostDialogueClick();
//    }

//}

using UnityEngine;

public class ShadowInteractive : Interactive
{
    public DialogueData_SO shadowDialogue;
    public Sprite newShadow1Sprite;
    public Sprite newShadow2Sprite;
    public GameObject pickupItemPrefab;

    public bool dialogueStarted = false;

    private void OnEnable()
    {
        EventHandler.DialogueFinishedEvent += OnDialogueFinished;
    }

    private void OnDisable()
    {
        EventHandler.DialogueFinishedEvent -= OnDialogueFinished;
    }

    private void OnDialogueFinished()
    {
        dialogueStarted = true;
        Debug.Log("对话结束");
    }

    // 重写点击方法，第一次点击开始对话
    public override void CheckItem(ItemName itemName)
    {
        if (!dialogueStarted && shadowDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(shadowDialogue);
        }
    }

    public override void EmptyClicked()
    {
        if (!dialogueStarted && shadowDialogue != null)
        {
            DialogueManager.Instance.StartDialogue(shadowDialogue);
        }
    }
}
