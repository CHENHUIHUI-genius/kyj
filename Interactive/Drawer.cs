using UnityEngine;

/// <summary>
/// 抽屉交互类：点击打开/关闭抽屉
/// </summary>
public class Drawer : Interactive
{
    public Sprite openSprite;       // 打开时的图片
    private SpriteRenderer spriteRenderer;
    private Sprite closedSprite;    // 关闭状态的原始图片
    private bool isOpen = false;    // 当前是否打开

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        closedSprite = spriteRenderer.sprite;  // 保存关闭状态的sprite
    }

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

    private void OnStartNewGameEvent()
    {
        // 重置状态
        isOpen = false;
        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        // 可以根据保存的状态加载，但目前简单重置
        if (isOpen && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closedSprite;
        }
    }

    // 重写空点击：直接切换打开/关闭状态
    public override void EmptyClicked()
    {
        ToggleDrawer();
    }

    // 重写CheckItem：如果有物品，也切换（可选）
    public override void CheckItem(ItemName itemName)
    {
        ToggleDrawer();
    }

    private void ToggleDrawer()
    {
        isOpen = !isOpen;
        if (isOpen && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closedSprite;
        }
    }
}
