using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform modelTransform;

    private static readonly Quaternion AxisCompensation = Quaternion.Euler(-90f, 0f, 0f);

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float h = 0f;
        float v = 0f;

        if (keyboard.dKey.isPressed) h += 1f;
        if (keyboard.aKey.isPressed) h -= 1f;
        if (keyboard.wKey.isPressed) v += 1f;
        if (keyboard.sKey.isPressed) v -= 1f;

        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        if (controller.isGrounded)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = Vector3.zero;

        if (inputDir.sqrMagnitude > 0.01f)
        {
            // Rotate the model child with axis compensation for FBX Z-up → Y-up
            if (modelTransform != null)
            {
                Quaternion targetRot = Quaternion.LookRotation(inputDir, Vector3.up) * AxisCompensation;
                modelTransform.rotation = Quaternion.RotateTowards(
                    modelTransform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            move = inputDir * moveSpeed;
        }

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }
}
