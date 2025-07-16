using UnityEngine;

public class ChangeSceneTrigger : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Changing scene to: {sceneName}");
            SceneLoader.Instance.LoadScene(sceneName);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Changing scene to: {sceneName}");
            SceneLoader.Instance.LoadScene(sceneName);
        }
    }
}
