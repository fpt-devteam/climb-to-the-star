using UnityEngine;

public class Footstep : MonoBehaviour
{
    public void PlayFootstep()
    {
        AudioManager.Instance.PlaySFX(AudioSFXEnum.PlayerMove);
    }

    public void StopFootstep()
    {
        AudioManager.Instance.StopSFX();
    }
}
