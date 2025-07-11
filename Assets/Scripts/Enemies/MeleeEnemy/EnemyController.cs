using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 2f;

    private StateMachine stateMachine;

    public StateMachine StateMachine => stateMachine;

    private void Awake()
    {
        stateMachine = new StateMachine();
        var patrolState = new EnemyPatrolState(this);

        stateMachine.SetState(patrolState);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void MoveTowards(Vector2 targetPosition)
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
    }
}
