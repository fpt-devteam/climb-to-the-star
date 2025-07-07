using UnityEngine;
using System.Collections.Generic;
using Player;

namespace Managers
{
    /// <summary>
    /// Manages player instances, spawning, and player-related game logic
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private bool _autoSpawnPlayer = true;

        [Header("Player Data")]
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private int _maxPlayers = 1;

        [Header("Spawn Settings")]
        [SerializeField] private float _respawnDelay = 2f;
        [SerializeField] private bool _respawnOnDeath = true;

        // Singleton pattern
        public static PlayerManager Instance { get; private set; }

        // Events
        public System.Action<PlayerController> OnPlayerSpawned;
        public System.Action<PlayerController> OnPlayerDied;
        public System.Action<PlayerController> OnPlayerRespawned;

        // Properties
        public PlayerController CurrentPlayer => _currentPlayer;
        public PlayerData PlayerData => _playerData;
        public bool HasPlayer => _currentPlayer != null;

        // Private fields
        private PlayerController _currentPlayer;
        private List<PlayerController> _allPlayers;
        private bool _isInitialized = false;

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton pattern implementation
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {

        }

        private void OnDestroy()
        {
            // Cleanup events
            OnPlayerSpawned = null;
            OnPlayerDied = null;
            OnPlayerRespawned = null;
        }

        #endregion

        #region Initialization

        public void Initialize()
        {
            if (_isInitialized) return;

            _allPlayers = new List<PlayerController>();
            
            // Find spawn point if not assigned
            if (_spawnPoint == null)
            {
                GameObject spawnPointGO = GameObject.FindGameObjectWithTag("PlayerSpawn");
                if (spawnPointGO != null)
                {
                    _spawnPoint = spawnPointGO.transform;
                }
                else
                {
                    // Create default spawn point
                    GameObject defaultSpawn = new GameObject("DefaultPlayerSpawn");
                    defaultSpawn.transform.position = Vector3.zero;
                    _spawnPoint = defaultSpawn.transform;
                }
            }

            // Load player data if not assigned
            if (_playerData == null)
            {
                _playerData = Resources.Load<PlayerData>("PlayerData");
                if (_playerData == null)
                {
                    Debug.LogWarning("PlayerData not found. Creating default player data.");
                    _playerData = CreateDefaultPlayerData();
                }
            }

            _isInitialized = true;
        }

        private PlayerData CreateDefaultPlayerData()
        {
            PlayerData defaultData = ScriptableObject.CreateInstance<PlayerData>();
            defaultData.Initialize();
            return defaultData;
        }

        #endregion

    }
}