using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Tooltip("Scrolling speed of the background (should be slower than ground for parallax depth)")]
    public float speed = 1.5f;

    [Tooltip("Width of one background tile in world units. If 0, auto-calculated from SpriteRenderer bounds.")]
    public float width = 0f;

    [Tooltip("The other background tile to snap behind when wrapping. If unassigned, automatically finds sibling.")]
    public Transform otherTile;

    private float tileWidth;

    private void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (width > 0f)
        {
            tileWidth = width;
        }
        else if (sr != null && sr.sprite != null)
        {
            tileWidth = sr.bounds.size.x;
        }
        else
        {
            tileWidth = 22f;
        }
    }

    private void Start()
    {
        // Automatically find sibling background tile if not explicitly assigned
        if (otherTile == null && transform.parent != null)
        {
            foreach (Transform sibling in transform.parent)
            {
                if (sibling != transform && sibling.GetComponent<BackgroundScroller>() != null)
                {
                    otherTile = sibling;
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        // Move left at parallax speed scaled with game speed
        float currentSpeed = (GameManager.Instance != null) ? speed * GameManager.Instance.SpeedMultiplier : speed;
        transform.position += Vector3.left * currentSpeed * Time.deltaTime;

        // When completely scrolled off-screen to the left
        if (transform.position.x <= -tileWidth)
        {
            if (otherTile != null)
            {
                // Snap seamlessly behind the other tile with a tiny sub-pixel overlap to prevent any seam gap
                transform.position = new Vector3(
                    otherTile.position.x + tileWidth - 0.02f,
                    transform.position.y,
                    transform.position.z
                );
            }
            else
            {
                transform.position += Vector3.right * (tileWidth * 2f);
            }
        }
    }
}
