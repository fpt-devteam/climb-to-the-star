using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    public float parallaxEffect;
    public Camera camera;
    private float startPoint;

    private void Start()
    {
        startPoint = transform.position.x;
    }

    private void FixedUpdate()
    {
        float distance = camera.transform.position.x * parallaxEffect;

        transform.position = new Vector3(
            startPoint + distance,
            transform.position.y,
            transform.position.z
        );
    }
}
