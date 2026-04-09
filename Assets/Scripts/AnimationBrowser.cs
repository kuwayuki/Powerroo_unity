using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AnimationBrowser : MonoBehaviour
{
    [SerializeField] private AnimationClip[] clips;
    [SerializeField] private RuntimeAnimatorController baseController;

    private int currentIndex = -1;
    private bool previewing;
    private bool showList;
    private bool showHud = true;
    private Vector2 scrollPos;
    private string jumpInput = "";
    private AnimatorOverrideController overrideController;
    private Animator animator;
    private WalkAnimation walkAnimation;
    private Text display2Text;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        walkAnimation = GetComponentInChildren<WalkAnimation>();
        CreateDisplay2UI();
    }

    private void CreateDisplay2UI()
    {
        var canvasObj = new GameObject("AnimBrowserCanvas_D2");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.targetDisplay = 1;
        canvasObj.AddComponent<CanvasScaler>();

        var textObj = new GameObject("Label");
        textObj.transform.SetParent(canvasObj.transform, false);

        display2Text = textObj.AddComponent<Text>();
        display2Text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        display2Text.fontSize = 24;
        display2Text.fontStyle = FontStyle.Bold;
        display2Text.color = Color.white;
        display2Text.alignment = TextAnchor.UpperLeft;

        var shadow = textObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.8f);
        shadow.effectDistance = new Vector2(1f, -1f);

        var rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(15f, -15f);
        rect.sizeDelta = new Vector2(500f, 80f);

        display2Text.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (showList) return; // OnGUI handles input when list is open

        var kb = Keyboard.current;
        if (kb == null || clips == null || clips.Length == 0) return;

        // O: next, Shift+O: previous
        if (kb.oKey.wasPressedThisFrame)
        {
            if (kb.leftShiftKey.isPressed)
                currentIndex = (currentIndex - 1 + clips.Length) % clips.Length;
            else
                currentIndex = (currentIndex + 1) % clips.Length;
            PlayClip(currentIndex);
        }

        // ] : +10, [ : -10
        if (kb.rightBracketKey.wasPressedThisFrame)
        {
            currentIndex = Mathf.Min(currentIndex + 10, clips.Length - 1);
            PlayClip(currentIndex);
        }
        if (kb.leftBracketKey.wasPressedThisFrame)
        {
            currentIndex = Mathf.Max(currentIndex - 10, 0);
            PlayClip(currentIndex);
        }

        // L: toggle list
        if (kb.lKey.wasPressedThisFrame)
            showList = !showList;

        // H: toggle HUD
        if (kb.hKey.wasPressedThisFrame)
            showHud = !showHud;

        // P: stop
        if (kb.pKey.wasPressedThisFrame && previewing)
            StopPreview();
    }

    private void PlayClip(int index)
    {
        if (animator == null || baseController == null) return;
        if (index < 0 || index >= clips.Length) return;

        if (!previewing)
        {
            previewing = true;
            if (walkAnimation != null)
                walkAnimation.enabled = false;

            animator.enabled = true;
            overrideController = new AnimatorOverrideController(baseController);
            animator.runtimeAnimatorController = overrideController;
        }

        currentIndex = index;
        overrideController["DummyPreview"] = clips[index];
        animator.Rebind();
        animator.Update(0f);
        animator.Play("Preview", 0, 0f);

        UpdateLabel();
    }

    private void StopPreview()
    {
        previewing = false;
        currentIndex = -1;
        showList = false;

        if (animator != null)
        {
            animator.enabled = false;
            animator.runtimeAnimatorController = null;
        }

        if (walkAnimation != null)
            walkAnimation.enabled = true;

        if (display2Text != null)
            display2Text.gameObject.SetActive(false);
    }

    private void UpdateLabel()
    {
        if (clips == null || currentIndex < 0) return;

        string label = string.Format("[{0}/{1}] {2}\nO/Shift+O: Prev/Next  [/]: +-10  L: List  P: Stop  H: Hide",
            currentIndex + 1, clips.Length, clips[currentIndex].name);

        if (display2Text != null)
        {
            display2Text.gameObject.SetActive(showHud);
            display2Text.text = label;
        }
    }

    private void OnGUI()
    {
        if (clips == null || clips.Length == 0) return;

        // Toggle button (always visible when previewing)
        if (previewing && currentIndex >= 0)
        {
            string btnText = showHud ? "H: Hide" : "H: Show";
            if (GUI.Button(new Rect(10, 10, 70, 25), btnText))
                showHud = !showHud;
        }

        // HUD
        if (previewing && currentIndex >= 0 && showHud)
        {
            string label = string.Format("[{0}/{1}] {2}\nO/Shift+O: Prev/Next  [/]: +-10  L: List  P: Stop  H: Hide",
                currentIndex + 1, clips.Length, clips[currentIndex].name);

            var style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            GUI.Label(new Rect(10, 40, 600, 60), label, style);
        }

        // List panel
        if (!showList) return;

        GUI.Box(new Rect(10, 70, 360, 500), "");

        // Jump input
        GUI.Label(new Rect(20, 75, 80, 25), "Jump to:");
        jumpInput = GUI.TextField(new Rect(90, 75, 60, 25), jumpInput);
        if (GUI.Button(new Rect(155, 75, 40, 25), "Go"))
        {
            if (int.TryParse(jumpInput, out int num))
            {
                num = Mathf.Clamp(num - 1, 0, clips.Length - 1);
                PlayClip(num);
            }
        }

        // Scrollable list
        scrollPos = GUI.BeginScrollView(
            new Rect(15, 105, 350, 460),
            scrollPos,
            new Rect(0, 0, 320, clips.Length * 22));

        var normalStyle = new GUIStyle(GUI.skin.button)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 13
        };
        var selectedStyle = new GUIStyle(normalStyle);
        selectedStyle.normal.textColor = Color.yellow;
        selectedStyle.fontStyle = FontStyle.Bold;

        for (int i = 0; i < clips.Length; i++)
        {
            bool isCurrent = i == currentIndex;
            string btnLabel = string.Format("{0}: {1}", i + 1, clips[i].name);
            var usedStyle = isCurrent ? selectedStyle : normalStyle;

            if (GUI.Button(new Rect(0, i * 22, 320, 21), btnLabel, usedStyle))
            {
                PlayClip(i);
            }
        }

        GUI.EndScrollView();
    }
}
