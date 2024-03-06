using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelLoader : MonoBehaviour
{
    private Button button;
    [SerializeField] private string LevelName;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(onClick);
    }
    
    private void onClick()
    {
        if(LevelName == "Lobby")
        {
            SoundManager.Instance.Play(Sounds.ButtonClick);
            SceneManager.LoadScene(LevelName);
        }
        else
        {
            LevelStatus levelStatus = LevelManager.Instance.GetLevelStatus(LevelName);
            switch (levelStatus)
            {
                case LevelStatus.Locked:
                    Debug.Log("Cant't play this level till you unlock it");
                    break;

                case LevelStatus.Unlocked:
                    LoadSceneAndPlaySound();
                    break;

                case LevelStatus.Completed:
                    LoadSceneAndPlaySound();
                    break;
            }
        }
    }

    public void LoadSceneAndPlaySound()
    {
        SoundManager.Instance.Play(Sounds.ButtonClick);
        SceneManager.LoadScene(LevelName);
        SoundManager.Instance.setVolumeOfAudioSource(SoundManager.Instance.soundMusic, 0.1f);
    }
}
