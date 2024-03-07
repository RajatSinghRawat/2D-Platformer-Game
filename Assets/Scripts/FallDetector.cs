using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FallDetector : MonoBehaviour
{
    [SerializeField] private Transform PlayerTransform;
    private Vector2 position;

    private void Awake()
    {
        position = transform.position;
    }
    private void Update()
    {
        position.x = PlayerTransform.position.x;
        transform.position = position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
