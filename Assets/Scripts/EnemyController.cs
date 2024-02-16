using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private Rigidbody2D EnemyRigidbody;


    private void Update()
    {
        EnemyRigidbody.velocity = new Vector2(Speed, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.KillPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Speed = -Speed;
        FlipEnemy();
    }

    private void FlipEnemy()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(EnemyRigidbody.velocity.x)), transform.localScale.y);
    }

}
