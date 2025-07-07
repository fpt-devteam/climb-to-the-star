using UnityEngine;

namespace Player
{
    /// <summary>
    /// ScriptableObject containing player data and statistics
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Player Statistics")]
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth = 100;
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _dashForce = 15f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _wallSlideSpeed = 2f;

        [Header("Game Progress")]
        [SerializeField] private int _level = 1;
        [SerializeField] private int _score = 0;
        [SerializeField] private int _coins = 0;
        [SerializeField] private float _playTime = 0f;
        [SerializeField] private int _deaths = 0;
        [SerializeField] private int _checkpointsReached = 0;

        [Header("Unlocks")]
        [SerializeField] private bool _hasDoubleJump = false;
        [SerializeField] private bool _hasDash = false;
        [SerializeField] private bool _hasWallJump = false;

        // Events
        public System.Action<int> OnHealthChanged;
        public System.Action<int> OnScoreChanged;
        public System.Action<int> OnCoinsChanged;
        public System.Action<int> OnLevelChanged;

        // Properties
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        public float MoveSpeed => _moveSpeed;
        public float JumpForce => _jumpForce;
        public float DashForce => _dashForce;
        public float DashDuration => _dashDuration;
        public float WallSlideSpeed => _wallSlideSpeed;
        public int Level => _level;
        public int Score => _score;
        public int Coins => _coins;
        public float PlayTime => _playTime;
        public int Deaths => _deaths;
        public int CheckpointsReached => _checkpointsReached;
        public bool HasDoubleJump => _hasDoubleJump;
        public bool HasDash => _hasDash;
        public bool HasWallJump => _hasWallJump;

        #region Initialization

        public void Initialize()
        {
            ResetToDefaults();
        }

        private void ResetToDefaults()
        {
            _currentHealth = _maxHealth;
            _level = 1;
            _score = 0;
            _coins = 0;
            _playTime = 0f;
            _deaths = 0;
            _checkpointsReached = 0;
            _hasDoubleJump = false;
            _hasDash = false;
            _hasWallJump = false;
        }

        #endregion

        #region Health Management

        public void SetHealth(int health)
        {
            int oldHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(health, 0, _maxHealth);
            
            if (oldHealth != _currentHealth)
            {
                OnHealthChanged?.Invoke(_currentHealth);
            }
        }

        public void TakeDamage(int damage)
        {
            SetHealth(_currentHealth - damage);
        }

        public void Heal(int amount)
        {
            SetHealth(_currentHealth + amount);
        }

        public void SetMaxHealth(int maxHealth)
        {
            _maxHealth = Mathf.Max(1, maxHealth);
            if (_currentHealth > _maxHealth)
            {
                SetHealth(_maxHealth);
            }
        }

        public bool IsDead => _currentHealth <= 0;

        #endregion

        #region Movement Stats

        public void SetMoveSpeed(float speed)
        {
            _moveSpeed = Mathf.Max(0f, speed);
        }

        public void SetJumpForce(float force)
        {
            _jumpForce = Mathf.Max(0f, force);
        }

        public void SetDashForce(float force)
        {
            _dashForce = Mathf.Max(0f, force);
        }

        public void SetDashDuration(float duration)
        {
            _dashDuration = Mathf.Max(0f, duration);
        }

        public void SetWallSlideSpeed(float speed)
        {
            _wallSlideSpeed = Mathf.Max(0f, speed);
        }

        #endregion

        #region Progress Management

        public void AddScore(int points)
        {
            int oldScore = _score;
            _score += points;
            
            if (oldScore != _score)
            {
                OnScoreChanged?.Invoke(_score);
            }
        }

        public void SetScore(int score)
        {
            int oldScore = _score;
            _score = Mathf.Max(0, score);
            
            if (oldScore != _score)
            {
                OnScoreChanged?.Invoke(_score);
            }
        }

        public void AddCoins(int amount)
        {
            int oldCoins = _coins;
            _coins += amount;
            
            if (oldCoins != _coins)
            {
                OnCoinsChanged?.Invoke(_coins);
            }
        }

        public void SetCoins(int coins)
        {
            int oldCoins = _coins;
            _coins = Mathf.Max(0, coins);
            
            if (oldCoins != _coins)
            {
                OnCoinsChanged?.Invoke(_coins);
            }
        }

        public void SetLevel(int level)
        {
            int oldLevel = _level;
            _level = Mathf.Max(1, level);
            
            if (oldLevel != _level)
            {
                OnLevelChanged?.Invoke(_level);
            }
        }

        public void IncrementLevel()
        {
            SetLevel(_level + 1);
        }

        public void AddPlayTime(float time)
        {
            _playTime += time;
        }

        public void SetPlayTime(float time)
        {
            _playTime = Mathf.Max(0f, time);
        }

        public void IncrementDeaths()
        {
            _deaths++;
        }

        public void SetDeaths(int deaths)
        {
            _deaths = Mathf.Max(0, deaths);
        }

        public void IncrementCheckpoints()
        {
            _checkpointsReached++;
        }

        public void SetCheckpointsReached(int checkpoints)
        {
            _checkpointsReached = Mathf.Max(0, checkpoints);
        }

        #endregion

        #region Unlocks

        public void UnlockDoubleJump()
        {
            _hasDoubleJump = true;
        }

        public void UnlockDash()
        {
            _hasDash = true;
        }

        public void UnlockWallJump()
        {
            _hasWallJump = true;
        }

        public void SetDoubleJumpUnlocked(bool unlocked)
        {
            _hasDoubleJump = unlocked;
        }

        public void SetDashUnlocked(bool unlocked)
        {
            _hasDash = unlocked;
        }

        public void SetWallJumpUnlocked(bool unlocked)
        {
            _hasWallJump = unlocked;
        }

        #endregion

        #region Data Persistence

        public void SaveData()
        {
            PlayerPrefs.SetInt("PlayerMaxHealth", _maxHealth);
            PlayerPrefs.SetInt("PlayerCurrentHealth", _currentHealth);
            PlayerPrefs.SetFloat("PlayerMoveSpeed", _moveSpeed);
            PlayerPrefs.SetFloat("PlayerJumpForce", _jumpForce);
            PlayerPrefs.SetFloat("PlayerDashForce", _dashForce);
            PlayerPrefs.SetFloat("PlayerDashDuration", _dashDuration);
            PlayerPrefs.SetFloat("PlayerWallSlideSpeed", _wallSlideSpeed);
            PlayerPrefs.SetInt("PlayerLevel", _level);
            PlayerPrefs.SetInt("PlayerScore", _score);
            PlayerPrefs.SetInt("PlayerCoins", _coins);
            PlayerPrefs.SetFloat("PlayerPlayTime", _playTime);
            PlayerPrefs.SetInt("PlayerDeaths", _deaths);
            PlayerPrefs.SetInt("PlayerCheckpointsReached", _checkpointsReached);
            PlayerPrefs.SetInt("PlayerHasDoubleJump", _hasDoubleJump ? 1 : 0);
            PlayerPrefs.SetInt("PlayerHasDash", _hasDash ? 1 : 0);
            PlayerPrefs.SetInt("PlayerHasWallJump", _hasWallJump ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void LoadData()
        {
            _maxHealth = PlayerPrefs.GetInt("PlayerMaxHealth", _maxHealth);
            _currentHealth = PlayerPrefs.GetInt("PlayerCurrentHealth", _currentHealth);
            _moveSpeed = PlayerPrefs.GetFloat("PlayerMoveSpeed", _moveSpeed);
            _jumpForce = PlayerPrefs.GetFloat("PlayerJumpForce", _jumpForce);
            _dashForce = PlayerPrefs.GetFloat("PlayerDashForce", _dashForce);
            _dashDuration = PlayerPrefs.GetFloat("PlayerDashDuration", _dashDuration);
            _wallSlideSpeed = PlayerPrefs.GetFloat("PlayerWallSlideSpeed", _wallSlideSpeed);
            _level = PlayerPrefs.GetInt("PlayerLevel", _level);
            _score = PlayerPrefs.GetInt("PlayerScore", _score);
            _coins = PlayerPrefs.GetInt("PlayerCoins", _coins);
            _playTime = PlayerPrefs.GetFloat("PlayerPlayTime", _playTime);
            _deaths = PlayerPrefs.GetInt("PlayerDeaths", _deaths);
            _checkpointsReached = PlayerPrefs.GetInt("PlayerCheckpointsReached", _checkpointsReached);
            _hasDoubleJump = PlayerPrefs.GetInt("PlayerHasDoubleJump", _hasDoubleJump ? 1 : 0) == 1;
            _hasDash = PlayerPrefs.GetInt("PlayerHasDash", _hasDash ? 1 : 0) == 1;
            _hasWallJump = PlayerPrefs.GetInt("PlayerHasWallJump", _hasWallJump ? 1 : 0) == 1;
        }

        public void ResetData()
        {
            ResetToDefaults();
            SaveData();
        }

        public void ClearSavedData()
        {
            PlayerPrefs.DeleteKey("PlayerMaxHealth");
            PlayerPrefs.DeleteKey("PlayerCurrentHealth");
            PlayerPrefs.DeleteKey("PlayerMoveSpeed");
            PlayerPrefs.DeleteKey("PlayerJumpForce");
            PlayerPrefs.DeleteKey("PlayerDashForce");
            PlayerPrefs.DeleteKey("PlayerDashDuration");
            PlayerPrefs.DeleteKey("PlayerWallSlideSpeed");
            PlayerPrefs.DeleteKey("PlayerLevel");
            PlayerPrefs.DeleteKey("PlayerScore");
            PlayerPrefs.DeleteKey("PlayerCoins");
            PlayerPrefs.DeleteKey("PlayerPlayTime");
            PlayerPrefs.DeleteKey("PlayerDeaths");
            PlayerPrefs.DeleteKey("PlayerCheckpointsReached");
            PlayerPrefs.DeleteKey("PlayerHasDoubleJump");
            PlayerPrefs.DeleteKey("PlayerHasDash");
            PlayerPrefs.DeleteKey("PlayerHasWallJump");
        }

        #endregion

        #region Utility Methods

        public float GetHealthPercentage()
        {
            return (float)_currentHealth / _maxHealth;
        }

        public bool IsFullHealth()
        {
            return _currentHealth >= _maxHealth;
        }

        public bool IsLowHealth()
        {
            return _currentHealth <= _maxHealth * 0.25f;
        }

        public string GetFormattedPlayTime()
        {
            int hours = Mathf.FloorToInt(_playTime / 3600f);
            int minutes = Mathf.FloorToInt((_playTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(_playTime % 60f);
            
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }

        #endregion
    }
} 