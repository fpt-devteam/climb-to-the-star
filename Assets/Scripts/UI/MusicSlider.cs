using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private void Awake()
    {
        slider.value = 1;
    }

    public void OnValueChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnToggle(bool value)
    {
        AudioManager.Instance.ToggleMusic();
    }
}
