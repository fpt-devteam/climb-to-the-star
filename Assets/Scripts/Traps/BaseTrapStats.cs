using UnityEngine;

public class BaseTrapStats : MonoBehaviour
{
  [Header("Trap Settings")]
  [SerializeField] private float damage = 10f;


  public virtual float GetDamage() => damage;
}