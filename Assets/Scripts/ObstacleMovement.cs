
using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    public float speed = 7f;
    public float destroyX = -12f;

    private void Update()
    {
        if (GameManager.Instance == null ||
            !GameManager.Instance.IsPlaying)
            return;

        float currentSpeed = (GameManager.Instance != null) ? GameManager.Instance.CurrentSpeed : speed;
        transform.position += Vector3.left *
            currentSpeed * Time.deltaTime;

        if (transform.position.x < destroyX)
            Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Obstacle touched: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit obstacle!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
            else
            {
                Debug.LogError("GameManager.Instance is NULL!");
            }
        }
    }

}
