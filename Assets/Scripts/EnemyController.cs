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
    [SerializeField] private AudioSource EnemyAudioSource;
    [SerializeField] private Animator EnemyAnimator;
    private Dictionary<Sounds, bool> isSoundClipPlaying;

    private float xOffset;
    private Vector2 startPosition, newPosition;             // Starting position and New position


    void Start()
    {
        isSoundClipPlaying = new Dictionary<Sounds, bool>();
        EnemyAudioSource = GetComponent<AudioSource>();

        // Save the starting position       
        startPosition = transform.position;
        if(!rightFlipped)
        {
            PatrollingDistance = -(PatrollingDistance);
        }

        EnemyAnimator = GetComponent<Animator>();

    }

    void Update()
    {

        // Calculate the new position based on sine wave

        xOffset = Mathf.Sin(Time.time * frequency) * PatrollingDistance;
        newPosition = startPosition + Vector2.right * xOffset;

        if (newPosition.x < transform.position.x && rightFlipped)
        {
            FlipEnemy();
            rightFlipped = false;
        }
        else if (newPosition.x > transform.position.x && !rightFlipped)
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
            float yNormal = collision.GetContact(0).normal.y;
            //Debug.Log(yNormal);
            Debug.Log("Enemy Collided");
            if(yNormal < -0.9f)
            {
                KillEnemy();
            }
            else
            {
                HandleEnemyAudio(Sounds.EnemyAttack);
                PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
                playerController.LooseOneLife();
            }
            //Debug.Log("Player Collided: " + collision.GetContact(0).normal.y);           
        }
    }

    private void FlipEnemy()
    {
        transform.localScale = new Vector2(-(transform.localScale.x), transform.localScale.y);
    }


    private IEnumerator SetClipIsNotPlaying(float duration, Sounds sound)
    {
        yield return new WaitForSeconds(duration);
        isSoundClipPlaying[sound] = false;
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject); // Destroy the enemy GameObject
    }

    private void HandleEnemyAudio(Sounds sound)
    {
        if (!isSoundClipPlaying.ContainsKey(sound))
        {
            isSoundClipPlaying.Add(sound, false);
        }

        if (isSoundClipPlaying[sound] == false)
        {
            float clipDuration = SoundManager.Instance.PlaySoundOfAudioSource(EnemyAudioSource, sound);
            isSoundClipPlaying[sound] = true;
            StartCoroutine(SetClipIsNotPlaying(clipDuration, sound));
        }
    }

    private void KillEnemy()
    {
        EnemyAnimator.SetBool("Dead", true);
        HandleEnemyAudio(Sounds.EnemyDeath);
        Invoke("DestroyEnemy", EnemyAnimator.GetCurrentAnimatorStateInfo(0).length);
    }
}
