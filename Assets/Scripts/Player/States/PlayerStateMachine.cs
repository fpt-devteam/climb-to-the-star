using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Manages player state transitions and current state execution
    /// </summary>
    public class PlayerStateMachine : MonoBehaviour
    {
        [Header("State Machine")]
        [SerializeField] private IPlayerState _currentState;
        [SerializeField] private IPlayerState _previousState;

        // Events
        public System.Action<IPlayerState> OnStateChanged;

        // Properties
        public IPlayerState CurrentState => _currentState;
        public IPlayerState PreviousState => _previousState;
        public string CurrentStateName => _currentState?.StateName ?? "None";

        #region Unity Lifecycle

        private void Update()
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.UpdateState();
                _currentState.HandleInput();
            }
        }

        private void FixedUpdate()
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.FixedUpdateState();
            }
        }

        #endregion

        #region State Management

        public void ChangeState(IPlayerState newState)
        {
            if (newState == null)
            {
                Debug.LogWarning("Attempting to change to null state!");
                return;
            }

            if (_currentState == newState)
            {
                Debug.LogWarning($"Attempting to change to same state: {newState.StateName}");
                return;
            }

            // Exit current state
            if (_currentState != null)
            {
                _currentState.ExitState();
            }

            // Store previous state
            _previousState = _currentState;

            // Set new state
            _currentState = newState;

            // Enter new state
            _currentState.EnterState();

            // Notify listeners
            OnStateChanged?.Invoke(_currentState);

            Debug.Log($"Player State Changed: {_previousState?.StateName ?? "None"} -> {_currentState.StateName}");
        }

        public void InitializeState(IPlayerState initialState)
        {
            if (initialState == null)
            {
                Debug.LogError("Cannot initialize with null state!");
                return;
            }

            _currentState = initialState;
            _currentState.EnterState();
            OnStateChanged?.Invoke(_currentState);

            Debug.Log($"Player State Initialized: {_currentState.StateName}");
        }

        public void ReturnToPreviousState()
        {
            if (_previousState != null)
            {
                ChangeState(_previousState);
            }
        }

        public bool IsInState<T>() where T : IPlayerState
        {
            return _currentState is T;
        }

        public bool IsInState(string stateName)
        {
            return _currentState?.StateName == stateName;
        }

        public T GetCurrentStateAs<T>() where T : class, IPlayerState
        {
            return _currentState as T;
        }

        #endregion

        #region Collision Handling

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.HandleCollision(collision);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.HandleCollision(collision);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.HandleCollision(collision);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.HandleTrigger(other);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.HandleTrigger(other);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (_currentState != null && !IsPaused())
            {
                _currentState.HandleTrigger(other);
            }
        }

        #endregion

        #region Utility Methods

        private bool IsPaused()
        {
            // Check if game is paused
            return Time.timeScale == 0f;
        }

        public void PauseStateMachine()
        {
            // State machine will automatically pause when Time.timeScale is 0
        }

        public void ResumeStateMachine()
        {
            // State machine will automatically resume when Time.timeScale is not 0
        }

        public void ForceState(IPlayerState newState)
        {
            // Force state change without normal validation
            if (newState == null) return;

            if (_currentState != null)
            {
                _currentState.ExitState();
            }

            _previousState = _currentState;
            _currentState = newState;
            _currentState.EnterState();
            OnStateChanged?.Invoke(_currentState);
        }

        #endregion

        #region Debug Methods

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void LogCurrentState()
        {
            Debug.Log($"Current Player State: {CurrentStateName}");
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void LogStateHistory()
        {
            Debug.Log($"Previous State: {_previousState?.StateName ?? "None"}");
            Debug.Log($"Current State: {CurrentStateName}");
        }

        #endregion
    }
} 