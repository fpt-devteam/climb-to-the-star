using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System.Collections;

public class BossEncounterTrigger : MonoBehaviour
{
  [Header("Boss Setup")]
  [SerializeField] private GameObject bossPrefab;
  [SerializeField] private Slider bossSlider;
  [SerializeField] private GameObject borderLeft;
  [SerializeField] private GameObject borderRight;
  [SerializeField] private GameObject bossCastEncounter;
  [SerializeField] private CinemachineVirtualCamera currentBossCamera;

  [Header("Audio Settings")]
  [SerializeField] private AudioMusicEnum bossMusic = AudioMusicEnum.Level5;

  [Header("Camera Settings")]
  [SerializeField] private float cameraOrthographicSize = 6f;
  [SerializeField] private bool lockCameraDuringBossFight = true;

  [Header("Encounter Settings")]
  [SerializeField] private float encounterStartDelay = 0.5f;
  [SerializeField] private bool disableTriggerAfterUse = true;

  private Transform originalFollowTarget;
  private Transform originalLookAtTarget;
  private Vector3 originalCameraPosition;
  private float originalOrthographicSize;
  private bool isBossFightActive = false;

  private bool encounterTriggered = false;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player") && !encounterTriggered)
    {
      encounterTriggered = true;

      Debug.Log("🔥 Boss encounter triggered!");
      StartCoroutine(StartBossEncounter());
    }
  }

  private IEnumerator StartBossEncounter()
  {
    SetupEncounterEnvironment();

    bossCastEncounter.SetActive(true);

    yield return new WaitForSeconds(encounterStartDelay);

    SpawnBoss();

    bossCastEncounter.SetActive(false);

    if (lockCameraDuringBossFight)
      SetCameraToFixedPosition();

    PlayBossMusic();

    CompleteEncounterSetup();

    Debug.Log("✅ Boss encounter fully initialized!");
  }

  private void SetupEncounterEnvironment()
  {
    if (bossSlider != null)
      bossSlider.gameObject.SetActive(true);
    // if (borderLeft != null)
    //   borderLeft.SetActive(true);
    // if (borderRight != null)
    //   borderRight.SetActive(true);
  }

  private void SpawnBoss()
  {
    if (bossPrefab != null)
    {
      bossPrefab.SetActive(true);
    }
  }

  public void SetCameraToFixedPosition()
  {
    // currentBossCamera.Follow = null;
    // currentBossCamera.LookAt = null;
    // currentBossCamera.m_Lens.OrthographicSize = cameraOrthographicSize;
    isBossFightActive = true;
    Debug.Log($"📹 Camera fixed at position: {currentBossCamera.transform.position}");
  }

  private void PlayBossMusic() => AudioManager.Instance.PlayMusic(bossMusic);
  private void CompleteEncounterSetup() => GetComponent<Collider2D>().enabled = false;
}