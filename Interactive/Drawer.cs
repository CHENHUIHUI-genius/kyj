using UnityEngine;

/// <summary>
/// 抽屉交互类：点击打开/关闭抽屉
/// </summary>
public class Drawer : Interactive, ISavable
{
    public Sprite openSprite;       // 打开时的图片
    public bool hasItemInside;      // 是否包含物品
    public int drawerIndex;         // 抽屉编号 (1-4)
    public AudioClip unlockSound;   // 解锁音效

    private SpriteRenderer spriteRenderer;
    private Sprite closedSprite;    // 关闭状态的原始图片
    private bool isOpen = false;    // 当前是否打开
    private bool isLocked = false;  // 是否锁定

    // 静态变量跟踪前三个抽屉状态和顺序
    private static bool drawer1Open = false;
    private static bool drawer2Open = false;
    private static bool drawer3Open = false;
    private static bool puzzleCompleted = false;
    private static int expectDrawer = 2; // 期望打开的抽屉：2->1->3
    private static bool opened2 = false; // 是否按顺序打开过2
    private static bool opened1 = false; // 是否按顺序打开过1
    private static bool opened3 = false; // 是否按顺序打开过3

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        closedSprite = spriteRenderer.sprite;  // 保存关闭状态的sprite

        // 第四个抽屉初始锁定
        if (drawerIndex == 4)
        {
            isLocked = true;
        }

        SavableRegister();
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
        isLocked = (drawerIndex == 4);  // 重新锁定第四个
        drawer1Open = false;
        drawer2Open = false;
        drawer3Open = false;
        puzzleCompleted = false;
        expectDrawer = 2;
        opened1 = false;
        opened2 = false;
        opened3 = false;

        if (spriteRenderer != null && closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
        // 隐藏物品
        if (hasItemInside && transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        // 恢复状态
        if (isOpen && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closedSprite;
        }
        // 显示/隐藏物品
        if (hasItemInside && transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(isOpen);
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
        Debug.Log($"尝试切换抽屉{drawerIndex}, isLocked={isLocked}, puzzleCompleted={puzzleCompleted}, 当前isOpen={isOpen}");

        // 检查是否锁定
        if (isLocked && drawerIndex == 4 && !puzzleCompleted)
        {
            Debug.Log($"抽屉{drawerIndex}已锁定，无法打开");
            return;
        }

        // 前三个抽屉可以随意打开
        bool wasOpen = isOpen;
        isOpen = !isOpen;
        Debug.Log($"抽屉{drawerIndex} 从 {wasOpen} 切换到 {isOpen}");

        if (isOpen && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closedSprite;
        }
        // 显示/隐藏物品
        if (hasItemInside && transform.childCount > 0)
        {
            transform.GetChild(0).gameObject.SetActive(isOpen);
        }

        // 更新静态状态
        UpdateStaticState();

        Debug.Log($"检查谜题: drawer1Open={drawer1Open}, drawer2Open={drawer2Open}, drawer3Open={drawer3Open}, puzzleCompleted={puzzleCompleted}");

        // 检查是否完成谜题（按顺序打开且所有前三个都打开）
        if (!puzzleCompleted && opened1 && opened2 && opened3 && drawer1Open && drawer2Open && drawer3Open)
        {
            puzzleCompleted = true;
            Debug.Log("谜题完成，解锁第四个抽屉");
            // 播放解锁音效
            Debug.Log($"播放音效检查: unlockSound = {unlockSound}");
            if (unlockSound != null)
            {
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);
                Debug.Log("音效已播放");
            }
            else
            {
                Debug.Log("音效文件未设置");
            }
            // 解锁第四个抽屉
            UnlockFourthDrawer();
        }
    }

    public void SavableRegister()
    {
        SaveLoadManager.Instance.Register(this);
    }

    public void GenerateSaveData(GameSaveData saveData)
    {
        // 保存每个抽屉的打开状态
        SetBoolData(saveData, $"drawer{drawerIndex}Open", isOpen);

        // 第四个抽屉保存谜题相关状态
        if (drawerIndex == 4)
        {
            SetBoolData(saveData, "puzzleCompleted", puzzleCompleted);
            SetBoolData(saveData, "opened1", opened1);
            SetBoolData(saveData, "opened2", opened2);
            SetBoolData(saveData, "opened3", opened3);
            // expectDrawer可以从opened推断
        }
    }

    public void RestoreGameData(GameSaveData saveData)
    {
        // 恢复打开状态
        isOpen = GetBoolData(saveData, $"drawer{drawerIndex}Open", false);
        // 更新sprite
        if (isOpen && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            spriteRenderer.sprite = closedSprite;
        }
        // 更新静态状态
        UpdateStaticState();

        // 恢复谜题状态
        if (drawerIndex == 4)
        {
            puzzleCompleted = GetBoolData(saveData, "puzzleCompleted", false);
            opened1 = GetBoolData(saveData, "opened1", false);
            opened2 = GetBoolData(saveData, "opened2", false);
            opened3 = GetBoolData(saveData, "opened3", false);
            // 重新计算expectDrawer
            if (!opened2) expectDrawer = 2;
            else if (!opened1) expectDrawer = 1;
            else if (!opened3) expectDrawer = 3;
            else expectDrawer = -1; // 已完成

            if (puzzleCompleted)
            {
                isLocked = false;
            }
        }
    }

    private void SetBoolData(GameSaveData saveData, string key, bool value)
    {
        int index = saveData.boolKeys.IndexOf(key);
        if (index >= 0)
        {
            saveData.boolValues[index] = value;
        }
        else
        {
            saveData.boolKeys.Add(key);
            saveData.boolValues.Add(value);
        }
    }

    private bool GetBoolData(GameSaveData saveData, string key, bool defaultValue = false)
    {
        int index = saveData.boolKeys.IndexOf(key);
        return index >= 0 ? saveData.boolValues[index] : defaultValue;
    }

    private void UpdateStaticState()
    {
        switch (drawerIndex)
        {
            case 1:
                drawer1Open = isOpen;
                // 检查顺序：只有打开时才检查
                if (isOpen && drawerIndex == expectDrawer)
                {
                    opened1 = true;
                    expectDrawer = 3; // 下一步期望3
                }
                break;
            case 2:
                drawer2Open = isOpen;
                if (isOpen && drawerIndex == expectDrawer)
                {
                    opened2 = true;
                    expectDrawer = 1; // 下一步期望1
                }
                break;
            case 3:
                drawer3Open = isOpen;
                if (isOpen && drawerIndex == expectDrawer)
                {
                    opened3 = true;
                }
                break;
        }
    }

    private void UnlockFourthDrawer()
    {
        // 找到第四个抽屉并解锁
        Drawer[] allDrawers = FindObjectsByType<Drawer>(FindObjectsSortMode.None);
        foreach (Drawer drawer in allDrawers)
        {
            if (drawer.drawerIndex == 4)
            {
                drawer.isLocked = false;
                break;
            }
        }
    }
}
