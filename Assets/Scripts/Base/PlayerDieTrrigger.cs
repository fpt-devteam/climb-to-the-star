using UnityEngine;

public class PlayerDieTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Player has died. Triggering game over.");
            GameManager.Instance.ChangeGameState(GameState.GameOver);
        }
    }
}
