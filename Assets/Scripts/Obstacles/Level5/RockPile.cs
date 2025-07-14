using UnityEngine;

public class RockPile : MonoBehaviour
{

    public void OnMeteorImpact(Vector3 meteorPosition)
    {
        Debug.Log($"[RockPile] {gameObject.name} bị meteor chạm trúng - Sẽ nổ!");
        Explode();
    }

    private void Explode()
    {
        Destroy(gameObject);
    }
}