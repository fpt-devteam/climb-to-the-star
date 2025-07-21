using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class BossEncounterTrigger : MonoBehaviour
{
  [Header("Boss Setup")]
  [SerializeField] private Slider bossSlider;


  [Header("Audio Settings")]
  [SerializeField] private AudioMusicEnum bossMusic = AudioMusicEnum.Level5;


  [Header("Camera Settings")]
  [SerializeField] private CinemachineVirtualCamera currentCamera;
  [SerializeField] private CinemachineVirtualCamera bossCamera;
  private CinemachineFramingTransposer framingTransposer;

  private void Awake()
  {
    framingTransposer = currentCamera.GetComponent<CinemachineFramingTransposer>();
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      AudioManager.Instance.PlayMusic(bossMusic);

      SwapCamera(bossCamera);

      GetComponent<Collider2D>().enabled = false;

      if (bossSlider != null) bossSlider.gameObject.SetActive(true);

    }
  }

  private void SwapCamera(CinemachineVirtualCamera newCamera)
  {
    currentCamera.enabled = false;
    newCamera.enabled = true;
    currentCamera = newCamera;
    framingTransposer = currentCamera.GetComponent<CinemachineFramingTransposer>();
  }
}

