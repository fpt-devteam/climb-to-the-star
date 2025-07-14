using UnityEngine;
using System.Collections.Generic;

public class Meteor : MonoBehaviour
{
    public GameObject explosionEffect;
    public float fallSpeed = 5f;
    public float horizontalSpeed = 2f;

    private Vector3 direction;
    private bool isExploded = false;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }

        direction = new Vector3(
            Random.Range(-horizontalSpeed, horizontalSpeed),
            -fallSpeed,
            0
        ).normalized * fallSpeed;
    }

    void Update()
    {
        if (!isExploded)
        {
            transform.position += direction * Time.deltaTime;

            if (mainCamera != null)
            {
                Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
                if (viewportPos.y < -0.1f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Rock"))
        {
            ExplodeBoth(collision.gameObject);
            isExploded = true;
        }
    }

    private void Explode()
    {
        Destroy(gameObject);
    }

    private void ExplodeBoth(GameObject target)
    {
        RockPile rockPile = target.GetComponent<RockPile>();

        if (rockPile != null)
        {
            rockPile.OnMeteorImpact(transform.position);
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, target.transform.position, Quaternion.identity);
        }
        Explode();
    }
}