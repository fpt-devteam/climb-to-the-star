using UnityEngine;

public class PlayAudioMusic : MonoBehaviour
{
  [SerializeField]
  private AudioMusicEnum audioMusic;

  private void Start()
  {
    Debug.Log("Playing music: " + audioMusic);
    AudioManager.Instance.PlayMusic(audioMusic);
  }
}
