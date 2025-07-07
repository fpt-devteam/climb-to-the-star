using UnityEngine;

namespace Player.States
{
    public abstract class BasePlayerState : IPlayerState
    {
        protected PlayerController _playerController;
        protected PlayerData _playerData;
        protected Rigidbody2D _rigidbody;
        protected Animator _animator;
        protected SpriteRenderer _spriteRenderer;

        public abstract string StateName { get; }

        public BasePlayerState(PlayerController playerController)
        {
            _playerController = playerController;
            _playerData = playerController.PlayerData;
            _rigidbody = playerController.GetComponent<Rigidbody2D>();
            _animator = playerController.GetComponent<Animator>();
            _spriteRenderer = playerController.GetComponent<SpriteRenderer>();
        }

        #region IPlayerState Implementation

        public virtual void EnterState()
        {
            // Override in derived classes
        }

        public virtual void UpdateState()
        {
            // Override in derived classes
        }

        public virtual void FixedUpdateState()
        {
            // Override in derived classes
        }

        public virtual void ExitState()
        {
            // Override in derived classes
        }

        public virtual void HandleInput()
        {
            // Override in derived classes
        }

        public virtual void HandleCollision(Collision2D collision)
        {
            // Override in derived classes
        }

        public virtual void HandleTrigger(Collider2D other)
        {
            // Override in derived classes
        }

        #endregion

        #region Common State Methods

        protected bool IsGrounded()
        {
            return _playerController.IsGrounded;
        }

        protected bool IsTouchingWall()
        {
            return _playerController.IsTouchingWall;
        }

        protected bool IsTouchingCeiling()
        {
            return _playerController.IsTouchingCeiling;
        }

        protected void SetAnimationTrigger(string triggerName)
        {
            if (_animator != null)
            {
                _animator.SetTrigger(triggerName);
            }
        }

        protected void SetAnimationBool(string boolName, bool value)
        {
            if (_animator != null)
            {
                _animator.SetBool(boolName, value);
            }
        }

        protected void SetAnimationFloat(string floatName, float value)
        {
            if (_animator != null)
            {
                _animator.SetFloat(floatName, value);
            }
        }

        protected void FlipSprite(bool facingRight)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.flipX = !facingRight;
            }
        }

        protected void MoveHorizontally(float direction)
        {
            if (_rigidbody != null)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity.x = direction * _playerData.MoveSpeed;
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void Jump()
        {
            if (_rigidbody != null)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity.y = _playerData.JumpForce;
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void WallJump(Vector2 direction)
        {
            if (_rigidbody != null)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity.x = direction.x * _playerData.JumpForce * 0.8f;
                velocity.y = direction.y * _playerData.JumpForce;
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void Dash(Vector2 direction)
        {
            if (_rigidbody != null)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity = direction * _playerData.DashForce;
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void WallSlide()
        {
            if (_rigidbody != null)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity.y = Mathf.Max(velocity.y, -_playerData.WallSlideSpeed);
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void StopHorizontalMovement()
        {
            if (_rigidbody != null)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity.x = 0f;
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void StopVerticalMovement()
        {
            if (_rigidbody != null)
            {
                Vector2 velocity = _rigidbody.linearVelocity;
                velocity.y = 0f;
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void StopAllMovement()
        {
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = Vector2.zero;
            }
        }

        protected bool IsInputEnabled()
        {
            return _playerController.IsInputEnabled;
        }

        protected bool IsPaused()
        {
            return _playerController.IsPaused;
        }

        protected float GetHorizontalInput()
        {
            return _playerController.HorizontalInput;
        }

        protected bool GetJumpInput()
        {
            return _playerController.JumpInput;
        }

        protected bool GetDashInput()
        {
            return _playerController.DashInput;
        }

        protected bool GetJumpInputDown()
        {
            return _playerController.JumpInputDown;
        }

        protected bool GetDashInputDown()
        {
            return _playerController.DashInputDown;
        }

        protected void ChangeState(IPlayerState newState)
        {
            _playerController.ChangeState(newState);
        }

        protected void PlaySound(AudioClip clip)
        {
            if (Managers.SoundManager.Instance != null && clip != null)
            {
                Managers.SoundManager.Instance.PlaySFX(clip);
            }
        }

        #endregion

        #region Utility Methods

        protected bool IsMovingRight()
        {
            return GetHorizontalInput() > 0f;
        }

        protected bool IsMovingLeft()
        {
            return GetHorizontalInput() < 0f;
        }

        protected bool IsMoving()
        {
            return Mathf.Abs(GetHorizontalInput()) > 0.1f;
        }

        protected bool IsFalling()
        {
            return _rigidbody != null && _rigidbody.linearVelocity.y < 0f;
        }

        protected bool IsRising()
        {
            return _rigidbody != null && _rigidbody.linearVelocity.y > 0f;
        }

        protected bool IsMovingHorizontally()
        {
            return _rigidbody != null && Mathf.Abs(_rigidbody.linearVelocity.x) > 0.1f;
        }

        protected float GetHorizontalVelocity()
        {
            return _rigidbody != null ? _rigidbody.linearVelocity.x : 0f;
        }

        protected float GetVerticalVelocity()
        {
            return _rigidbody != null ? _rigidbody.linearVelocity.y : 0f;
        }

        protected Vector2 GetVelocity()
        {
            return _rigidbody != null ? _rigidbody.linearVelocity : Vector2.zero;
        }

        protected void SetVelocity(Vector2 velocity)
        {
            if (_rigidbody != null)
            {
                _rigidbody.linearVelocity = velocity;
            }
        }

        protected void AddForce(Vector2 force, ForceMode2D forceMode = ForceMode2D.Force)
        {
            if (_rigidbody != null)
            {
                _rigidbody.AddForce(force, forceMode);
            }
        }

        #endregion
    }
} 