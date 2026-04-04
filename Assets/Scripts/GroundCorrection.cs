using UnityEngine;

public class GroundCorrection : MonoBehaviour
{
    [SerializeField] private Transform modelTransform;
    [SerializeField] private string legMeshName = "leg_l_mesh";

    private CharacterController controller;
    private Animator animator;
    private Renderer legRenderer;
    private float baseLocalY;
    private float feetOffset;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (modelTransform == null)
            modelTransform = transform.Find("Powerroo_born");
        if (modelTransform != null)
        {
            animator = modelTransform.GetComponent<Animator>();
            baseLocalY = modelTransform.localPosition.y;
            CacheLegRenderer();
        }
    }

    private void Start()
    {
        if (legRenderer != null)
            feetOffset = legRenderer.bounds.min.y - transform.position.y;
    }

    private void CacheLegRenderer()
    {
        foreach (var r in modelTransform.GetComponentsInChildren<Renderer>())
        {
            if (r.gameObject.name == legMeshName)
            {
                legRenderer = r;
                return;
            }
        }
    }

    private void LateUpdate()
    {
        if (modelTransform == null || legRenderer == null) return;

        if (animator == null || !animator.enabled || animator.runtimeAnimatorController == null)
        {
            modelTransform.localPosition = new Vector3(
                modelTransform.localPosition.x, baseLocalY, modelTransform.localPosition.z);
            return;
        }

        float currentCorrection = baseLocalY - modelTransform.localPosition.y;
        float feetY = legRenderer.bounds.min.y;
        float targetFeetY = transform.position.y + feetOffset;
        float totalCorrection = currentCorrection + (feetY - targetFeetY);

        modelTransform.localPosition = new Vector3(
            modelTransform.localPosition.x,
            baseLocalY - totalCorrection,
            modelTransform.localPosition.z
        );
    }
}
