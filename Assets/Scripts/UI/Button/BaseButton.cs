using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Managers;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class BaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
      [Header("Change Scene")]
      [SerializeField] private bool changeSceneOnClick = false;
      [SerializeField] private string sceneNameToLoad = "";

      [Header("Sound")]
      [SerializeField] private bool playSoundOnClick = true;
      [SerializeField] private AudioClip customClickSound;

      [Header("Animation")]
      [SerializeField] private Animator buttonAnimator;
      [SerializeField] private string hoverAnimationTrigger = "Hover";
      [SerializeField] private string clickAnimationTrigger = "Click";

      private Button button;

      private void Awake()
      {
          button = GetComponent<Button>();

          if (buttonAnimator == null)
          {
              buttonAnimator = GetComponent<Animator>();
          }
      }

      private void OnEnable()
      {
          button.onClick.AddListener(OnClick);
      }

      private void OnDisable()
      {
          button.onClick.RemoveListener(OnClick);
      }

      private void OnClick()
      {
          if (playSoundOnClick)
          {
              SoundManager.Instance.PlaySFX(customClickSound);
          }

          if (buttonAnimator != null && !string.IsNullOrEmpty(clickAnimationTrigger))
          {
              buttonAnimator.SetTrigger(clickAnimationTrigger);
          }

          if (changeSceneOnClick && !string.IsNullOrEmpty(sceneNameToLoad))
          {
              SceneLoader.Instance.LoadScene(sceneNameToLoad);
          }
      }

      public void OnPointerEnter(PointerEventData eventData)
      {
          if (buttonAnimator != null && !string.IsNullOrEmpty(hoverAnimationTrigger))
          {
              buttonAnimator.SetTrigger(hoverAnimationTrigger);
          }
      }
      
      public void OnPointerExit(PointerEventData eventData)
      {
      }
    }
} 