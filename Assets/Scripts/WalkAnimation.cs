using UnityEngine;

public class WalkAnimation : MonoBehaviour
{
    [Header("Walk")]
    [SerializeField] private float swingAngle = 30f;
    [SerializeField] private float swingSpeed = 8f;
    [SerializeField] private float tailSwingAngle = 20f;
    [SerializeField] private float tailSwingSpeed = 6f;

    [Header("Run")]
    [SerializeField] private float runSwingAngle = 45f;
    [SerializeField] private float runSwingSpeed = 14f;
    [SerializeField] private float runTailSwingAngle = 30f;

    [Header("Transition")]
    [SerializeField] private float smoothReturnSpeed = 8f;
    [SerializeField] private float stateBlendSpeed = 5f;

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
    private bool isRunning;
    private float phase;
    private float runBlend; // 0 = walk, 1 = run

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    public void SetRunning(bool running)
    {
        isRunning = running;
    }

    [Header("Bone Paths")]
    [SerializeField] private string armLPath = "CharacterArmature/root/body/shoulder_l/arm_l";
    [SerializeField] private string armRPath = "CharacterArmature/root/body/shoulder_r/arm_r";
    [SerializeField] private string legLPath = "CharacterArmature/root/leg_l";
    [SerializeField] private string legRPath = "CharacterArmature/root/leg_r";
    [SerializeField] private string tailPath = "CharacterArmature/root/body/tail";

    private void Awake()
    {
        armL = transform.Find(armLPath);
        armR = transform.Find(armRPath);
        legL = transform.Find(legLPath);
        legR = transform.Find(legRPath);
        tail = transform.Find(tailPath);

        if (armL != null) armLRest = armL.localRotation;
        if (armR != null) armRRest = armR.localRotation;
        if (legL != null) legLRest = legL.localRotation;
        if (legR != null) legRRest = legR.localRotation;
        if (tail != null) tailRest = tail.localRotation;
    }

    private void Update()
    {
        // Blend between walk and run
        float targetBlend = isRunning ? 1f : 0f;
        runBlend = Mathf.MoveTowards(runBlend, targetBlend, stateBlendSpeed * Time.deltaTime);

        float currentAngle = Mathf.Lerp(swingAngle, runSwingAngle, runBlend);
        float currentSpeed = Mathf.Lerp(swingSpeed, runSwingSpeed, runBlend);
        float currentTailAngle = Mathf.Lerp(tailSwingAngle, runTailSwingAngle, runBlend);

        if (isMoving)
        {
            phase += Time.deltaTime * currentSpeed;

            float swing = Mathf.Sin(phase);

            if (legL != null)
                legL.localRotation = legLRest * Quaternion.Euler(swing * currentAngle, 0f, 0f);
            if (legR != null)
                legR.localRotation = legRRest * Quaternion.Euler(-swing * currentAngle, 0f, 0f);

            if (armL != null)
                armL.localRotation = armLRest * Quaternion.Euler(-swing * currentAngle * 0.7f, 0f, 0f);
            if (armR != null)
                armR.localRotation = armRRest * Quaternion.Euler(swing * currentAngle * 0.7f, 0f, 0f);

            float tailSway = Mathf.Sin(phase * (tailSwingSpeed / currentSpeed));
            if (tail != null)
                tail.localRotation = tailRest * Quaternion.Euler(0f, tailSway * currentTailAngle, 0f);
        }
        else
        {
            float t = smoothReturnSpeed * Time.deltaTime;
            if (legL != null) legL.localRotation = Quaternion.Slerp(legL.localRotation, legLRest, t);
            if (legR != null) legR.localRotation = Quaternion.Slerp(legR.localRotation, legRRest, t);
            if (armL != null) armL.localRotation = Quaternion.Slerp(armL.localRotation, armLRest, t);
            if (armR != null) armR.localRotation = Quaternion.Slerp(armR.localRotation, armRRest, t);
            if (tail != null) tail.localRotation = Quaternion.Slerp(tail.localRotation, tailRest, t);

            phase = 0f;
            runBlend = 0f;
        }
    }
}
