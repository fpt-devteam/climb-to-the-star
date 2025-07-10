public interface ISkill
{
    void Initialize(Player player);
    void Execute();
    void Update();
    bool IsExecutable();
}
