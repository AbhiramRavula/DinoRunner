
using UnityEngine;

public class DinoAnimationController : MonoBehaviour
{
    public Sprite[] runFrames;
    public Sprite jumpFrame;
    public Sprite[] duckFrames;

    [Header("Animation Speeds")]
    [Tooltip("Seconds per frame. Higher number = SLOWER frame changes (e.g. 0.25 is slower than 0.10)")]
    [Range(0.05f, 0.5f)]
    public float runFrameRate = 0.22f;

    [Tooltip("Seconds per duck frame")]
    [Range(0.05f, 0.5f)]
    public float duckFrameRate = 0.15f;

    [Tooltip("Global playback speed multiplier. Lower = SLOWER (e.g., 0.6 = slow motion, 1.0 = normal)")]
    [Range(0.2f, 2.0f)]
    public float animationSpeedMultiplier = 1.0f;

    [Header("Scaling Adjustments")]
    [Tooltip("Multiplier applied to the Dino scale during jump to match the running sprite size")]
    public float jumpScaleMultiplier = 1.65f;

    private SpriteRenderer spriteRenderer;
    private DinoController dinoController;

    private int frameIndex;
    private float frameTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dinoController = GetComponent<DinoController>();
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        // Duck animation
        if (dinoController != null &&
            dinoController.IsDucking &&
            duckFrames != null &&
            duckFrames.Length > 0)
        {
            AnimateFrames(duckFrames, duckFrameRate);
            return;
        }

        // Jump pose: remain in jump frame whenever airborne
        if (dinoController != null && !dinoController.IsGrounded)
        {
            if (jumpFrame != null)
                spriteRenderer.sprite = jumpFrame;
            return;
        }

        // Running animation
        if (runFrames != null && runFrames.Length > 0)
        {
            AnimateFrames(runFrames, runFrameRate);
        }
    }

    private void AnimateFrames(Sprite[] frames, float frameRate)
    {
        if (frames == null || frames.Length == 0)
            return;

        // Accelerate animation speed proportionally with game speed
        float gameSpeedMultiplier = (GameManager.Instance != null) ? GameManager.Instance.SpeedMultiplier : 1f;
        float totalMultiplier = Mathf.Max(0.1f, animationSpeedMultiplier * gameSpeedMultiplier);
        float effectiveRate = frameRate / totalMultiplier;

        frameTimer += Time.deltaTime;

        if (frameTimer >= effectiveRate)
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % frames.Length;
        }

        // Clamp index to safe bounds
        if (frameIndex < 0 || frameIndex >= frames.Length)
            frameIndex = 0;

        if (frames[frameIndex] != null)
            spriteRenderer.sprite = frames[frameIndex];
    }
}
