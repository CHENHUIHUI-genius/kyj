using UnityEngine;

// 先继承Interactive
public class Box : Interactive
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Sprite closedSprite;    // 保存关闭状态的原始图片

    public Sprite openSprite;       // 打开时需要切换一下图片

    // 点击一次后就不希望该mailbox再次被点击
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        // 保存关闭状态的原始图片
        closedSprite = spriteRenderer.sprite;
    }

    // 判断paper该不该显示？也就是场景被加载之后
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    // 新游戏开始时重置Box状态
    private void OnStartNewGameEvent()
    {
        Debug.Log($"重置Box: {gameObject.name}");
        // 重置完成状态
        isDone = false;
        // 恢复关闭状态的原始图片
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
        // 启用碰撞体
        if (coll != null)
        {
            coll.enabled = true;
        }
        // 隐藏子物体（如果有的话）
        if (transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
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
