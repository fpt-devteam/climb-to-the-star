using UnityEngine;

namespace Player.States
{
    /// <summary>
    /// Interface defining the contract for all player states
    /// </summary>
    public interface IPlayerState
    {
        /// <summary>
        /// Called when entering this state
        /// </summary>
        void EnterState();

        /// <summary>
        /// Called every frame while in this state
        /// </summary>
        void UpdateState();

        /// <summary>
        /// Called every fixed update while in this state (for physics)
        /// </summary>
        void FixedUpdateState();

        /// <summary>
        /// Called when exiting this state
        /// </summary>
        void ExitState();

        /// <summary>
        /// Called when input is received while in this state
        /// </summary>
        void HandleInput();

        /// <summary>
        /// Called when collision occurs while in this state
        /// </summary>
        void HandleCollision(Collision2D collision);

        /// <summary>
        /// Called when trigger collision occurs while in this state
        /// </summary>
        void HandleTrigger(Collider2D other);

        /// <summary>
        /// The name of this state for debugging
        /// </summary>
        string StateName { get; }
    }
} 