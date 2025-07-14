using UnityEngine;

public class PlayAudioSFX : MonoBehaviour
{
  [SerializeField]
  private AudioSFXEnum audioSFX;

  private void Start()
  {
    AudioManager.Instance.PlaySFX(audioSFX);
  }
}
