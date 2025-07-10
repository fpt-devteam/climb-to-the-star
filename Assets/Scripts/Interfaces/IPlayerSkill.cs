public interface IPlayerSkill
{
    void Initialize(Player player);
    void Execute();
    bool IsExecutable();
    void Update();
}
