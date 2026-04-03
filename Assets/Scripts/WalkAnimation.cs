using UnityEngine;

public class WalkAnimation : MonoBehaviour
{
    [SerializeField] private float swingAngle = 30f;
    [SerializeField] private float swingSpeed = 8f;
    [SerializeField] private float tailSwingAngle = 20f;
    [SerializeField] private float tailSwingSpeed = 6f;
    [SerializeField] private float smoothReturnSpeed = 8f;

    private Transform armL;
    private Transform armR;
    private Transform legL;
    private Transform legR;
    private Transform tail;

    private Quaternion armLRest;
    private Quaternion armRRest;
    private Quaternion legLRest;
    private Quaternion legRRest;
    private Quaternion tailRest;

    private bool isMoving;
    private float phase;

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    private void Awake()
    {
        // Find bones by path
        armL = transform.Find("root/body/arm_l");
        armR = transform.Find("root/body/arm_r");
        legL = transform.Find("root/leg_l");
        legR = transform.Find("root/leg_r");
        tail = transform.Find("root/body/tail");

        // Store rest poses
        if (armL != null) armLRest = armL.localRotation;
        if (armR != null) armRRest = armR.localRotation;
        if (legL != null) legLRest = legL.localRotation;
        if (legR != null) legRRest = legR.localRotation;
        if (tail != null) tailRest = tail.localRotation;
    }

    private void Update()
    {
        if (isMoving)
        {
            phase += Time.deltaTime * swingSpeed;

            float swing = Mathf.Sin(phase);

            // Legs swing forward/backward (opposite to each other)
            if (legL != null)
                legL.localRotation = legLRest * Quaternion.Euler(swing * swingAngle, 0f, 0f);
            if (legR != null)
                legR.localRotation = legRRest * Quaternion.Euler(-swing * swingAngle, 0f, 0f);

            // Arms swing opposite to same-side leg
            if (armL != null)
                armL.localRotation = armLRest * Quaternion.Euler(-swing * swingAngle * 0.7f, 0f, 0f);
            if (armR != null)
                armR.localRotation = armRRest * Quaternion.Euler(swing * swingAngle * 0.7f, 0f, 0f);

            // Tail sways side to side
            float tailSway = Mathf.Sin(phase * (tailSwingSpeed / swingSpeed));
            if (tail != null)
                tail.localRotation = tailRest * Quaternion.Euler(0f, tailSway * tailSwingAngle, 0f);
        }
        else
        {
            // Smoothly return to rest pose
            float t = smoothReturnSpeed * Time.deltaTime;
            if (legL != null) legL.localRotation = Quaternion.Slerp(legL.localRotation, legLRest, t);
            if (legR != null) legR.localRotation = Quaternion.Slerp(legR.localRotation, legRRest, t);
            if (armL != null) armL.localRotation = Quaternion.Slerp(armL.localRotation, armLRest, t);
            if (armR != null) armR.localRotation = Quaternion.Slerp(armR.localRotation, armRRest, t);
            if (tail != null) tail.localRotation = Quaternion.Slerp(tail.localRotation, tailRest, t);

            phase = 0f;
        }
    }
}
