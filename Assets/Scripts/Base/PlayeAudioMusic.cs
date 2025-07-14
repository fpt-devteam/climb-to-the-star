using UnityEngine;

public class PlayAudioMusic : MonoBehaviour
{
    [SerializeField]
    private AudioMusicEnum audioMusic;

    private void Awake()
    {
        AudioManager.Instance.PlayMusic(audioMusic);
    }
}
