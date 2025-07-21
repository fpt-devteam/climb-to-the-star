using UnityEngine;

public class RestartButton : BaseButton
{
  protected override void HandleClick()
  {
    GameManager.Instance.RestartGame();
  }
}
