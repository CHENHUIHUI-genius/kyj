public interface ISavable
{
    // 注册到存档管理器
    void SavableRegister();

    // 生成存档数据（设置到传入的saveData中）
    void GenerateSaveData(GameSaveData saveData);

    // 从存档恢复
    void RestoreGameData(GameSaveData saveData);
}
