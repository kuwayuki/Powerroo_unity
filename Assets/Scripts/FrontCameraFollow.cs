using UnityEngine;

public class FrontCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float height = 0.3f;
    [SerializeField] private float smoothSpeed = 8f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + new Vector3(0f, height, 0f);
        Vector3 forward = target.forward;

        // Position camera in front of the character
        Vector3 desiredPos = targetPos + forward * distance;
        desiredPos.y = targetPos.y;

        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.LookAt(targetPos);
    }
}
