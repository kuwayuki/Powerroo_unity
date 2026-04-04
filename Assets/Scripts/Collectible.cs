using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController victoryController;
    [SerializeField] private float danceDuration = 10f;

    private bool collected;
    private Vector3 playerStartPos;
    private Quaternion modelStartRot;

    private void Start()
    {
        var player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            playerStartPos = player.transform.position;
            var model = player.transform.Find("Powerroo_born");
            if (model != null)
                modelStartRot = model.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        var player = other.GetComponent<PlayerMovement>();
        if (player == null) return;

        collected = true;
        StartCoroutine(VictorySequence(player));
    }

    private IEnumerator VictorySequence(PlayerMovement player)
    {
        // Hide gift box (keep GameObject active for coroutine)
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;

        // Stop movement and procedural animation
        player.enabled = false;
        var walkAnim = player.GetComponentInChildren<WalkAnimation>();
        if (walkAnim != null)
            walkAnim.enabled = false;

        // Teleport to start position (disable CharacterController to move)
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = playerStartPos;
        var model = player.transform.Find("Powerroo_born");
        if (model != null)
            model.rotation = modelStartRot;
        if (cc != null) cc.enabled = true;

        // Start victory dance
        var animator = player.GetComponentInChildren<Animator>();
        if (animator != null && victoryController != null)
        {
            animator.enabled = true;
            animator.runtimeAnimatorController = victoryController;
            animator.Rebind();
            animator.Update(0f);
            animator.SetBool("Victory", true);
        }

        // Dance for specified duration
        yield return new WaitForSeconds(danceDuration);

        // Stop dance
        if (animator != null)
        {
            animator.SetBool("Victory", false);
            animator.enabled = false;
            animator.runtimeAnimatorController = null;
        }

        // Reset model pose
        if (model != null)
            model.rotation = modelStartRot;

        // Re-enable gameplay
        if (walkAnim != null)
            walkAnim.enabled = true;
        player.enabled = true;

        // Re-show gift box
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = true;
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = true;

        collected = false;
    }
}
