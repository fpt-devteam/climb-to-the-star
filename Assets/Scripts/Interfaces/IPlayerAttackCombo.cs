public interface IPlayerAttackCombo
{
    void Initialize(PlayerStats playerStats);
    void HandleAttack(float baseDamage);
    int GetCurrentComboIndex();
}
