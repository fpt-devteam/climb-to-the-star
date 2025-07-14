using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private void Awake()
    {
        slider.value = 1;
    }

    public void OnValueChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    public void OnToggle(bool value)
    {
        AudioManager.Instance.ToggleSFX();
    }
}
