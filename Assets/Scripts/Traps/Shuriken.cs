using System.Collections.Generic;
using UnityEngine;

public class Shuriken : BaseTrapStats
{
  [SerializeField] private float speed = 10f;
  [SerializeField] private bool canMove = true;
  [SerializeField] private ShurikenMovementPath pointLst;
  private bool isMovingRight = true;
  private int currentPointIndex = 0;

  void Update()
  {
    if (!canMove) return;
    if (pointLst.movementPoints.Count < 2) return;

    if (IsReachedPoint(pointLst.movementPoints[currentPointIndex]))
    {
      currentPointIndex = (currentPointIndex + 1) % pointLst.movementPoints.Count;
    }

    var targetPoint = pointLst.movementPoints[currentPointIndex];

    Vector2 currentPosition = transform.position;
    Vector2 direction = ((Vector2)targetPoint - currentPosition).normalized;

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

    transform.position = Vector2.MoveTowards(currentPosition, targetPoint, speed * Time.deltaTime);
  }

  private bool IsReachedPoint(Vector2 point)
  {
    return Vector2.Distance(transform.position, point) < 0.1f;
  }
}