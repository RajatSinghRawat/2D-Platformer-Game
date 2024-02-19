using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private float speed;                   // Movement speed
    [SerializeField] private float PatrollingDistance;      // Amplitude of the sine wave
    [SerializeField] private float frequency;               // Frequency of the sine wave
    [SerializeField] private bool rightFlipped;
    
    private float xOffset;
    private Vector2 startPosition, newPosition;             // Starting position and New position

    void Start()
    {
        // Save the starting position       
        startPosition = transform.position;
        if(!rightFlipped)
        {
            PatrollingDistance = -(PatrollingDistance);
        }
    }

    void Update()
    {
        // Calculate the new position based on sine wave

        xOffset =  Mathf.Sin(Time.time * frequency) * PatrollingDistance;       
        newPosition = startPosition + Vector2.right * xOffset;

        if (newPosition.x < transform.position.x && rightFlipped)
        {
            FlipEnemy();
            rightFlipped = false;
        }
        else if(newPosition.x > transform.position.x && !rightFlipped)
        {
            FlipEnemy();
            rightFlipped = true;
        }
        
        // Move the enemy towards the new position
        transform.position = Vector2.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.LooseOneLife();
        }
    }

    private void FlipEnemy()
    {
        transform.localScale = new Vector2(-(transform.localScale.x), transform.localScale.y);
    }
}
