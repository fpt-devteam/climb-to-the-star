using UnityEngine;

public class BaseButton : MonoBehaviour
{
    public void OnClick()
    {
        AudioManager.Instance.PlaySFX(AudioSFXEnum.ButtonClick);
        HandleClick();
    }

    protected virtual void HandleClick() { }
}
