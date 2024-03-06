using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float FadeOutAnimationTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            SoundManager.Instance.Play(Sounds.Collectible);
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            playerController.PickUpKey();
            gameObject.GetComponentInChildren<BoxCollider2D>().enabled = false;
            animator.SetBool("Fade_Out", true);
            StartCoroutine("DestroyAfterAnimationFinished");
        }
    }

    private IEnumerator DestroyAfterAnimationFinished()
    {       
        yield return new WaitForSeconds(FadeOutAnimationTime);
        Destroy(transform.parent.gameObject);
    }
}
