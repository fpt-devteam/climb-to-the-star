using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;
using TMPro;

public class BossEncounterTrigger : MonoBehaviour
{
  [Header("Boss Setup")]
  [SerializeField] private Slider bossSlider;
  [SerializeField] private TextMeshProUGUI bossText;
  [SerializeField] private GameObject boss;




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
      GetComponent<Collider2D>().enabled = false;
      bossText.gameObject.SetActive(true);
      boss.SetActive(true);

      SwapCamera(bossCamera);
      if (bossSlider != null) bossSlider.gameObject.SetActive(true);

      StartCoroutine(WaitForPlayer(collision));
    }
  }

  private IEnumerator WaitForPlayer(Collider2D collision)
  {
    yield return new WaitForSeconds(2f);
    bossText.gameObject.SetActive(false);
  }

  private void SwapCamera(CinemachineVirtualCamera newCamera)
  {
    currentCamera.enabled = false;
    newCamera.enabled = true;
    currentCamera = newCamera;
    framingTransposer = currentCamera.GetComponent<CinemachineFramingTransposer>();
  }
}
