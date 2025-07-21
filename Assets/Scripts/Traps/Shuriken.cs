using System.Collections.Generic;
using UnityEngine;

public class Shuriken : BaseTrapStats
{
  [SerializeField] private float speed = 10f;
  [SerializeField] private GameObject leftPoint;
  [SerializeField] private GameObject rightPoint;
  private bool isMovingRight = true;

  void Update()
  {
    if (leftPoint == null || rightPoint == null) return;

    var currentTargetPoint = isMovingRight ? rightPoint : leftPoint;

    if (IsReachedPoint(currentTargetPoint.transform.position))
    {
      isMovingRight = !isMovingRight;
    }

    if (!isMovingRight && IsReachedPoint(leftPoint.transform.position))
    {
      currentTargetPoint = rightPoint;
      isMovingRight = true;
    }

    var targetPosition = currentTargetPoint.transform.position;

    Vector2 currentPosition = transform.position;
    Vector2 direction = ((Vector2)targetPosition - currentPosition).normalized;

    if (direction.x > 0 && !isMovingRight)
    {
      isMovingRight = true;
      transform.localScale = new Vector3(1, 1, 1);
    }
    else if (direction.x < 0 && isMovingRight)
    {
      isMovingRight = false;
      transform.localScale = new Vector3(-1, 1, 1);
    }

    transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);
  }

  private bool IsReachedPoint(Vector2 point)
  {
    return Vector2.Distance(transform.position, point) < 0.1f;
  }
}
