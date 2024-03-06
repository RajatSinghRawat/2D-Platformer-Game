using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOverController : MonoBehaviour
{
    public LevelCompleteController levelCompleteController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<PlayerController>() != null)
        {
            collision.GetComponent<PlayerController>().PlayerInactive();
            SoundManager.Instance.Play(Sounds.ButtonClick);
            LevelManager.Instance.MarkCurrentLevelComplete();
            levelCompleteController.LevelComplete();
        }
    }
}
