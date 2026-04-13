using UnityEngine;
using UnityEngine.InputSystem;

public class FaceExpression : MonoBehaviour
{
    [SerializeField] private string headBonePath = "CharacterArmature/root/body/head";
    [SerializeField] private string eyesNormalPath = "CharacterArmature/root/body/head/eyes_normal";
    [SerializeField] private string mouthClosePath = "CharacterArmature/root/body/head/mouth_close";
    [SerializeField] private string eyesSmileName = "eyes_smile";
    [SerializeField] private string mouthOpenName = "mouth_open";

    private GameObject eyesNormal;
    private GameObject mouthClose;
    private GameObject eyesSmile;
    private GameObject mouthOpen;

    private Transform headBone;
    private Transform smileParent;
    private Matrix4x4 eyesSmileOffset;
    private Matrix4x4 mouthOpenOffset;
    private bool isSmiling;
    private bool initialized;

    private void Awake()
    {
        var model = transform.Find("Powerroo_born");
        if (model == null) return;

        headBone = model.Find(headBonePath);
        Transform eyesNormalT = model.Find(eyesNormalPath);
        Transform mouthCloseT = model.Find(mouthClosePath);
        Transform eyesSmileT = model.Find(eyesSmileName);
        Transform mouthOpenT = model.Find(mouthOpenName);

        if (headBone == null || eyesNormalT == null || mouthCloseT == null) return;

        eyesNormal = eyesNormalT.gameObject;
        mouthClose = mouthCloseT.gameObject;

        if (eyesSmileT != null)
        {
            eyesSmile = eyesSmileT.gameObject;
            smileParent = eyesSmileT.parent;
            // Capture rest-pose relationship: where smile mesh is relative to head bone
            eyesSmileOffset = headBone.worldToLocalMatrix * eyesSmileT.localToWorldMatrix;
        }

        if (mouthOpenT != null)
        {
            mouthOpen = mouthOpenT.gameObject;
            // Capture rest-pose relationship
            mouthOpenOffset = headBone.worldToLocalMatrix * mouthOpenT.localToWorldMatrix;
        }

        initialized = true;
        ApplyExpression();
    }

    private void Update()
    {
        if (!initialized) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.qKey.wasPressedThisFrame)
        {
            isSmiling = !isSmiling;
            ApplyExpression();
            // Immediately update tracking to avoid 1-frame pop
            if (isSmiling) UpdateSmileTracking();
        }
    }

    private void LateUpdate()
    {
        if (!initialized || !isSmiling) return;
        UpdateSmileTracking();
    }

    private void UpdateSmileTracking()
    {
        if (headBone == null || smileParent == null) return;

        // Compute desired world matrix from current head bone + rest-pose offset
        if (eyesSmile != null)
        {
            Matrix4x4 desiredWorld = headBone.localToWorldMatrix * eyesSmileOffset;
            Matrix4x4 localMatrix = smileParent.worldToLocalMatrix * desiredWorld;
            ApplyMatrix(eyesSmile.transform, localMatrix);
        }

        if (mouthOpen != null)
        {
            Matrix4x4 desiredWorld = headBone.localToWorldMatrix * mouthOpenOffset;
            Matrix4x4 localMatrix = smileParent.worldToLocalMatrix * desiredWorld;
            ApplyMatrix(mouthOpen.transform, localMatrix);
        }
    }

    private static void ApplyMatrix(Transform target, Matrix4x4 m)
    {
        target.localPosition = m.GetColumn(3);

        Vector3 scale = new Vector3(
            m.GetColumn(0).magnitude,
            m.GetColumn(1).magnitude,
            m.GetColumn(2).magnitude
        );

        if (m.determinant < 0f)
            scale.x = -scale.x;

        target.localScale = scale;
        target.localRotation = Quaternion.LookRotation(
            m.GetColumn(2) / scale.z,
            m.GetColumn(1) / scale.y
        );
    }

    private void ApplyExpression()
    {
        if (eyesNormal != null) eyesNormal.SetActive(!isSmiling);
        if (mouthClose != null) mouthClose.SetActive(!isSmiling);
        if (eyesSmile != null) eyesSmile.SetActive(isSmiling);
        if (mouthOpen != null) mouthOpen.SetActive(isSmiling);
    }
}
