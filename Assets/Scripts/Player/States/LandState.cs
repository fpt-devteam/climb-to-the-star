using System.Collections;
using UnityEngine;

public class LandState : BasePlayerState
{
  private Animator animator;
  private bool isAppliedAnimation = false;
  private IPlayerMovement movement;
  private IPlayerInput input;
  private float landDuration;
  private bool isRolling = false;

  public LandState(PlayerController context)
      : base(context)
  {
    animator = context.GetComponent<Animator>();
    movement = context.PlayerMovement;
    input = context.PlayerInput;
  }

  public override void Enter()
  {
    // Notify movement system about landing
    movement.OnLanding();

    // Determine landing type based on movement
    isRolling = movement.ShouldRollOnLanding();

    if (isRolling)
    {
      // Rolling land - shorter duration, allows movement
      animator.Play("Roll"); // You'll need this animation, or use "Land" for now
      landDuration = 0.15f; // Quick roll
      AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerLand);
    }
    else
    {
      // Standing land - slightly longer, more stable
      animator.Play("Land");
      landDuration = 0.25f; // Stable landing
      AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerLand);
    }

    isAppliedAnimation = false;
    context.StartCoroutine(ExitToDefault());
  }

  public override void FixedUpdate()
  {
    if (isRolling)
    {
      // During roll, allow full movement for momentum conservation
      movement.Move(input.GetMovementInput());
    }
    else
    {
      // During standing land, reduce movement for stability
      float reducedInput = input.GetMovementInput() * 0.3f;
      movement.Move(reducedInput);
    }
  }

  private IEnumerator ExitToDefault()
  {
    yield return new WaitForSeconds(landDuration);
    isAppliedAnimation = true;
  }

  public override IState CheckTransitions()
  {
    if (isRolling)
    {
      // Rolling allows immediate actions for fluid gameplay
      if (context.IsJumping())
      {
        return context.GetState(PlayerState.Air); // Use unified AirState
      }

      if (context.CanAttack())
      {
        return context.GetState(PlayerState.Attack1);
      }

      if (context.IsDashing())
      {
        return context.GetState(PlayerState.Dash);
      }
    }
    else
    {
      // Standing land requires completion before most actions
      if (context.IsJumping() && isAppliedAnimation)
      {
        return context.GetState(PlayerState.Air); // Use unified AirState
      }
    }

    if (context.IsHurt())
    {
      return context.GetState(PlayerState.Hurt);
    }

    if (!isAppliedAnimation)
    {
      return null;
    }

    return context.GetState(PlayerState.Locomotion);
  }
}
