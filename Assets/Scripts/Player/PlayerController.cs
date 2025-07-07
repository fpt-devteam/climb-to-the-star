using UnityEngine;
using Player.States;

namespace Player
{
    /// <summary>
    /// Manages player movement, animations, and interactions
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private PlayerStateMachine _stateMachine;

        [Header("Ground Detection")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _groundLayerMask = 1;

        [Header("Wall Detection")]
        [SerializeField] private Transform _wallCheck;
        [SerializeField] private float _wallCheckDistance = 0.2f;
        [SerializeField] private LayerMask _wallLayerMask = 1;

        [Header("Ceiling Detection")]
        [SerializeField] private Transform _ceilingCheck;
        [SerializeField] private float _ceilingCheckRadius = 0.2f;
        [SerializeField] private LayerMask _ceilingLayerMask = 1;

        [Header("Input Settings")]
        [SerializeField] private string _horizontalInputName = "Horizontal";
        [SerializeField] private string _jumpInputName = "Jump";
        [SerializeField] private string _dashInputName = "Dash";

        // Events
        public System.Action OnPlayerDied;

        // Properties
        public PlayerData PlayerData { get; private set; }
        public bool IsGrounded { get; private set; }
        public bool IsTouchingWall { get; private set; }
        public bool IsTouchingCeiling { get; private set; }
        public bool IsInputEnabled { get; private set; } = true;
        public bool IsPaused { get; private set; } = false;
        public float HorizontalInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool DashInput { get; private set; }
        public bool JumpInputDown { get; private set; }
        public bool DashInputDown { get; private set; }

        // Private fields
        private bool _isInitialized = false;

        #region Unity Lifecycle

        private void Awake()
        {
            // Get components if not assigned
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody2D>();
            
            if (_animator == null)
                _animator = GetComponent<Animator>();
            
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (_stateMachine == null)
                _stateMachine = GetComponent<PlayerStateMachine>();

            // Create ground check if not assigned
            if (_groundCheck == null)
            {
                GameObject groundCheckGO = new GameObject("GroundCheck");
                groundCheckGO.transform.SetParent(transform);
                groundCheckGO.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                _groundCheck = groundCheckGO.transform;
            }

            // Create wall check if not assigned
            if (_wallCheck == null)
            {
                GameObject wallCheckGO = new GameObject("WallCheck");
                wallCheckGO.transform.SetParent(transform);
                wallCheckGO.transform.localPosition = new Vector3(0.5f, 0f, 0f);
                _wallCheck = wallCheckGO.transform;
            }

            // Create ceiling check if not assigned
            if (_ceilingCheck == null)
            {
                GameObject ceilingCheckGO = new GameObject("CeilingCheck");
                ceilingCheckGO.transform.SetParent(transform);
                ceilingCheckGO.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                _ceilingCheck = ceilingCheckGO.transform;
            }
        }

        private void Start()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        private void Update()
        {
            if (!IsPaused)
            {
                HandleInput();
            }
        }

        private void FixedUpdate()
        {
            if (!IsPaused)
            {
                UpdatePhysicsDetection();
            }
        }

        #endregion

        #region Initialization

        public void Initialize()
        {
            // if (_isInitialized) return;

            // // Initialize player data
            // PlayerData = ScriptableObject.CreateInstance<PlayerData>();
            // PlayerData.Initialize(); 

            // // Initialize state machine
            // if (_stateMachine != null)
            // {
            //     _stateMachine.InitializeState(new IdleState(this));
            // }

            // _isInitialized = true;
        }

        public void Initialize(PlayerData playerData)
        {
            // if (_isInitialized) return;

            // PlayerData = playerData;
            
            // // Initialize state machine
            // if (_stateMachine != null)
            // {
            //     _stateMachine.InitializeState(new IdleState(this));
            // }

            // _isInitialized = true;
        }

        #endregion

        #region Input Handling

        private void HandleInput()
        {
            if (!IsInputEnabled) return;

            // Get input values
            HorizontalInput = Input.GetAxis(_horizontalInputName);
            JumpInput = Input.GetButton(_jumpInputName);
            DashInput = Input.GetButton(_dashInputName);
            JumpInputDown = Input.GetButtonDown(_jumpInputName);
            DashInputDown = Input.GetButtonDown(_dashInputName);
        }

        #endregion

        #region Physics Detection

        private void UpdatePhysicsDetection()
        {
            // Ground detection
            IsGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayerMask);

            // Wall detection
            IsTouchingWall = Physics2D.Raycast(_wallCheck.position, Vector2.right, _wallCheckDistance, _wallLayerMask) ||
                             Physics2D.Raycast(_wallCheck.position, Vector2.left, _wallCheckDistance, _wallLayerMask);

            // Ceiling detection
            IsTouchingCeiling = Physics2D.OverlapCircle(_ceilingCheck.position, _ceilingCheckRadius, _ceilingLayerMask);
        }

        #endregion

        #region State Management

        public void ChangeState(IPlayerState newState)
        {
            if (_stateMachine != null)
            {
                _stateMachine.ChangeState(newState);
            }
        }

        public void UpdatePlayerData(PlayerData newPlayerData)
        {
            PlayerData = newPlayerData;
        }

        #endregion

        #region Control Methods

        public void SetInputEnabled(bool enabled)
        {
            IsInputEnabled = enabled;
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
        }

        #endregion

        #region Event Handling

        public void OnDeath()
        {
            OnPlayerDied?.Invoke();
        }

        #endregion

        #region Debug Methods

        private void OnDrawGizmosSelected()
        {
            // Draw ground check
            if (_groundCheck != null)
            {
                Gizmos.color = IsGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
            }

            // Draw wall check
            if (_wallCheck != null)
            {
                Gizmos.color = IsTouchingWall ? Color.green : Color.red;
                Gizmos.DrawRay(_wallCheck.position, Vector2.right * _wallCheckDistance);
                Gizmos.DrawRay(_wallCheck.position, Vector2.left * _wallCheckDistance);
            }

            // Draw ceiling check
            if (_ceilingCheck != null)
            {
                Gizmos.color = IsTouchingCeiling ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_ceilingCheck.position, _ceilingCheckRadius);
            }
        }

        #endregion
    }
}