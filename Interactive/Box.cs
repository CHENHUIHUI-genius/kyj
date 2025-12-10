using UnityEngine;

// 先继承Interactive
public class Box : Interactive
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;

    public Sprite openSprite;       // 打开时需要切换一下图片

    // 点击一次后就不希望该mailbox再次被点击
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    // 判断paper该不该显示？也就是场景被加载之后
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        // 首先判断互动是否已经完成，如果没有完成，默认需要将mailpiece隐藏起来，
        if (!isDone)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        // 互动已经完成，图片需要切换，且碰撞体也要关闭
        else
        {
            spriteRenderer.sprite = openSprite;
            coll.enabled = false;
        }
    }

    // 打开时需要切换一下图片
    protected override void OnClickedAction()
    {
        spriteRenderer.sprite = openSprite;
        transform.GetChild(0).gameObject.SetActive(true);   // 打开box后需要显示paper1
    }
}
