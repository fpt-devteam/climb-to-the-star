using System.Collections;
using UnityEngine;

public class HurtState : BasePlayerState
{
  private Animator animator;
  private float currentHealth;
  private bool isAppliedAnimation = false;
  private bool isAlive = true;
  private bool wasInAirWhenHurt = false; // Track if player was in air when hurt

  public HurtState(PlayerController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
  }

  public override void Enter()
  {
    currentHealth = context.PlayerStats.CurrentHealth;

    if (currentHealth <= 0)
    {
      isAlive = false;
      return;
    }

    // CRITICAL FIX: Track if player was in air when hurt
    wasInAirWhenHurt = !context.IsGrounded;
    Debug.Log($"=== HURT STATE ENTERED ===");
    Debug.Log($"Was in air when hurt: {wasInAirWhenHurt}");
    Debug.Log($"Current grounded state: {context.IsGrounded}");

    animator.Play("Hurt");
    AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerHurt);
  }

  public override IState CheckTransitions()
  {
    if (!isAlive)
    {
      return context.GetState(PlayerState.Die);
    }

    if (!context.IsHurt())
    {
      // CRITICAL FIX: Return to appropriate state based on air/ground status
      if (wasInAirWhenHurt && !context.IsGrounded)
      {
        Debug.Log("Hurt state complete - returning to Air state (was hurt in air)");
        return context.GetState(PlayerState.Air);
      }
      else if (context.IsGrounded)
      {
        Debug.Log("Hurt state complete - returning to Locomotion (on ground)");
        return context.GetState(PlayerState.Locomotion);
      }
      else
      {
        // Fallback: check if player is falling
        Debug.Log("Hurt state complete - checking fall state");
        if (context.IsFalling())
        {
          return context.GetState(PlayerState.Air);
        }
        else
        {
          return context.GetState(PlayerState.Locomotion);
        }
      }
    }

    return null;
  }
}
