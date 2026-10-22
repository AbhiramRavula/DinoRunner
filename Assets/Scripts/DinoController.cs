
using UnityEngine;

public class DinoController : MonoBehaviour
{
    public float jumpForce = 12f;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    public bool IsGrounded { get; private set; } = true;
    public bool IsDucking { get; private set; }

    private Vector2 standingSize;
    private Vector2 standingOffset;
    private float lockedX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        standingSize = boxCollider.size;
        standingOffset = boxCollider.offset;
        lockedX = transform.position.x;
    }

    private void FixedUpdate()
    {
        // Strictly prevent horizontal drift: zero out horizontal velocity and lock X position
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        transform.position = new Vector3(lockedX, transform.position.y, transform.position.z);
    }

    private Vector2 touchStartPos;
    private bool isTouchDucking;

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        // While ascending in a jump, we cannot be grounded
        if (rb != null && rb.linearVelocity.y > 0.1f)
        {
            IsGrounded = false;
        }

        bool jumpPressed =
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.UpArrow);

        // --- MOBILE TOUCH HANDLING (iOS & Android) ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Don't register gameplay jump/duck if tapping on a UI button (e.g. Pause / Settings)
            bool isOverUI = UnityEngine.EventSystems.EventSystem.current != null &&
                            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId);

            if (!isOverUI)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                    isTouchDucking = false;
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    float deltaY = touch.position.y - touchStartPos.y;

                    // Swiping downwards ducks the Dino
                    if (deltaY < -40f)
                    {
                        isTouchDucking = true;
                    }
                    // Swiping upwards jumps the Dino
                    else if (deltaY > 50f)
                    {
                        jumpPressed = true;
                        isTouchDucking = false;
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    // A quick tap (not a downward swipe) triggers a jump
                    float deltaY = touch.position.y - touchStartPos.y;
                    if (!isTouchDucking && Mathf.Abs(deltaY) < 40f)
                    {
                        jumpPressed = true;
                    }

                    isTouchDucking = false;
                }
            }
        }
        else
        {
            // Desktop mouse click fallback
            if (Input.GetMouseButtonDown(0))
            {
                bool isOverUI = UnityEngine.EventSystems.EventSystem.current != null &&
                                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
                if (!isOverUI)
                {
                    jumpPressed = true;
                }
            }
        }

        bool duckPressed =
            Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.S) ||
            isTouchDucking;

        IsDucking = duckPressed && IsGrounded;

        // Shorten the collider while ducking.
        if (IsDucking)
        {
            boxCollider.size = new Vector2(
                standingSize.x, standingSize.y * 0.55f);

            boxCollider.offset = new Vector2(
                standingOffset.x,
                standingOffset.y -
                standingSize.y * 0.225f);
        }
        else
        {
            boxCollider.size = standingSize;
            boxCollider.offset = standingOffset;
        }

        // Jump only when standing on the ground.
        if (jumpPressed && IsGrounded && !IsDucking)
        {
            rb.linearVelocity = new Vector2(0f, jumpForce);
            IsGrounded = false;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayJump();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckGrounded(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckGrounded(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            IsGrounded = false;
    }

    private void CheckGrounded(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
            return;

        // Refuse to ground if moving upwards
        if (rb != null && rb.linearVelocity.y > 0.1f)
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Only surfaces underneath the Dino count as ground
            if (contact.normal.y > 0.1f)
            {
                IsGrounded = true;
                return;
            }
        }
    }
}
