using UnityEngine;

public class PanelTriggerButton : BaseButton
{
  [Header("Panel Trigger")]
  [SerializeField]
  private bool openPanelOnClick = false;
  [SerializeField]
  private bool makePanelActive = true;

  [SerializeField]
  private GameObject panelToOpen;
  protected override void HandleClick()
  {
    if (openPanelOnClick && panelToOpen != null)
    {
      panelToOpen.SetActive(makePanelActive);
    }
  }
}
