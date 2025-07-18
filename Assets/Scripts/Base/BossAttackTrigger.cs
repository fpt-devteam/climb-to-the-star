using UnityEngine;
using System.Collections;
public class BossAttackTrigger : MonoBehaviour
{
  [SerializeField] private BossController bossController;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      var player = collision.GetComponent<PlayerStats>();
      player.TakeDamage(bossController.BossStats.MeleeAttackDamage);
      CameraShake.PlayerHurt();
    }
  }
}