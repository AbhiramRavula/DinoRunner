
using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float speed = 7f;
    public float tileWidth = 25f;

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        transform.position += Vector3.left *
            speed * Time.deltaTime;

        if (transform.position.x <= -tileWidth)
        {
            transform.position +=
                Vector3.right * tileWidth * 2f;
        }
    }
}
