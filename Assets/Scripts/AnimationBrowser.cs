using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationBrowser : MonoBehaviour
{
    [SerializeField] private AnimationClip[] clips;
    [SerializeField] private RuntimeAnimatorController baseController;

    private int currentIndex = -1;
    private bool previewing;
    private AnimatorOverrideController overrideController;
    private Animator animator;
    private WalkAnimation walkAnimation;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        walkAnimation = GetComponentInChildren<WalkAnimation>();
    }

    private void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || clips == null || clips.Length == 0) return;

        if (kb.oKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % clips.Length;
            PlayClip(currentIndex);
        }

        if (kb.pKey.wasPressedThisFrame && previewing)
        {
            StopPreview();
        }
    }

    private void PlayClip(int index)
    {
        if (animator == null || baseController == null) return;

        if (!previewing)
        {
            previewing = true;
            if (walkAnimation != null)
                walkAnimation.enabled = false;

            animator.enabled = true;

            overrideController = new AnimatorOverrideController(baseController);
            animator.runtimeAnimatorController = overrideController;
        }

        overrideController["DummyPreview"] = clips[index];
        animator.Rebind();
        animator.Update(0f);
        animator.Play("Preview", 0, 0f);
    }

    private void StopPreview()
    {
        previewing = false;
        currentIndex = -1;

        if (animator != null)
        {
            animator.enabled = false;
            animator.runtimeAnimatorController = null;
        }

        if (walkAnimation != null)
            walkAnimation.enabled = true;
    }

    private void OnGUI()
    {
        if (!previewing || clips == null || currentIndex < 0) return;

        string label = string.Format("[{0}/{1}] {2}\nO: Next  P: Stop",
            currentIndex + 1, clips.Length, clips[currentIndex].name);

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        GUI.Label(new Rect(10, 10, 500, 80), label, style);
    }
}
