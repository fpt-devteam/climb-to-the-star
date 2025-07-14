using UnityEngine;

public class BackgroundMoving : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 lastTargetPosition;
    private Transform cameraTransform;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not set for background parallax effect");
        }

        cameraTransform = transform;
        lastTargetPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 deltaMovement = new(
            cameraTransform.position.x - lastTargetPosition.x,
            cameraTransform.position.y - lastTargetPosition.y,
            cameraTransform.position.z - lastTargetPosition.z
            );
        target.position = new Vector3(
            target.position.x + deltaMovement.x * 0.7f,
            target.position.y + deltaMovement.y * 0.7f,
            target.position.z);

        lastTargetPosition = cameraTransform.position;
    }
}