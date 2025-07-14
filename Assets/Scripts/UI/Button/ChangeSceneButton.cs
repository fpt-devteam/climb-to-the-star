using UnityEngine;

public class ChangeSceneButton : BaseButton
{
  [Header("Change Scene")]
  [SerializeField]
  private bool changeSceneOnClick = false;

  [SerializeField]
  private string sceneNameToLoad = "";

  protected override void HandleClick()
  {
    if (changeSceneOnClick && !string.IsNullOrEmpty(sceneNameToLoad))
    {
      SceneLoader.Instance.LoadScene(sceneNameToLoad);
    }
  }
}
