using UnityEngine;

public class RotateItem : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float bobAmplitude = 0.15f;
    [SerializeField] private float bobFrequency = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        float yOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = startPosition + new Vector3(0f, yOffset, 0f);
    }
}
