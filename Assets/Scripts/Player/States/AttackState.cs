using System.Collections;
using UnityEngine;

public class AttackState : BasePlayerState
{
    private static readonly int[] AnimHashes =
    {
        Animator.StringToHash("Attack_1"),
        Animator.StringToHash("Attack_2"),
        Animator.StringToHash("Attack_3"),
        Animator.StringToHash("Attack_4"),
    };

    private static readonly float[] Durations = { 0.5f, 0.5f, 0.5f, 0.5f };

    private Animator animator;
    private GameObject attackPoint;

    private int attackIndex;
    private bool buffered;
    private float attackTimer;

    public AttackState(PlayerController playerController)
        : base(playerController)
    {
        animator = playerController.GetComponent<Animator>();
        attackPoint = playerController.AttackPoint;
    }

    public override void OnEnter()
    {
        attackIndex = 0;
        buffered = false;
        attackTimer = Durations[attackIndex];
        PlayCurrent();
        Debug.Log("Attacking");
    }

    public override void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f && playerController.IsAttacking())
        {
            buffered = true;
        }
    }

    public override void FixedUpdate()
    {
        if (attackTimer <= 0f)
        {
            if (buffered && attackIndex < 3)
            {
                attackIndex++;
                attackTimer = Durations[attackIndex];
                buffered = false;
                PlayCurrent();
            }
            else
            {
                playerController.stateMachine.SetState(new IdleState(playerController));
            }
        }
    }

    private void PlayCurrent()
    {
        animator.Play(AnimHashes[attackIndex]);
    }
}
