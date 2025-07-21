
using UnityEngine;
using UnityEngine.UI;
public class VolumeSetting : MonoBehaviour
{
  [SerializeField] private Slider musicSlider;
  [SerializeField] private Slider sfxSlider;

  private void Start()
  {
    musicSlider.value = AudioManager.Instance.GetMusicVolume();
    sfxSlider.value = AudioManager.Instance.GetSFXVolume();
  }

  public void OnMusicVolumeChanged()
  {
    Debug.Log("OnMusicVolumeChanged: " + musicSlider.value);
    AudioManager.Instance.SetMusicVolume(musicSlider.value);
  }

  public void OnSFXVolumeChanged()
  {
    Debug.Log("OnSFXVolumeChanged: " + sfxSlider.value);
    AudioManager.Instance.SetSFXVolume(sfxSlider.value);
  }
}

