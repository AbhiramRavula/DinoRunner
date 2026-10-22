
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public Sprite[] flyingFrames;
    public float speed = 7f;
    public float frameRate = 0.12f;
    public float destroyX = -12f;

    private SpriteRenderer spriteRenderer;
    private int frameIndex;
    private float frameTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        // Fly from right to left.
        transform.position += Vector3.left *
            speed * Time.deltaTime;

        // Animate the wing flaps.
        if (flyingFrames != null &&
            flyingFrames.Length > 0)
        {
            frameTimer += Time.deltaTime;

            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) %
                    flyingFrames.Length;

                spriteRenderer.sprite =
                    flyingFrames[frameIndex];
            }
        }

        if (transform.position.x < destroyX)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
