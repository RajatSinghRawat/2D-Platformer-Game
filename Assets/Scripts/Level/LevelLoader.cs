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
            SoundManager.Instance.PlayMusic(Sounds.Music);
            LoadSceneAndPlaySound(Sounds.ButtonClick);
            SetMusicVolume(0.5f);
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
                    LoadSceneAndPlaySound(Sounds.ButtonClick);
                    SetMusicVolume(0.1f);
                    SoundManager.Instance.Play(Sounds.NewLevelStart);
                    break;

                case LevelStatus.Completed:
                    LoadSceneAndPlaySound(Sounds.ButtonClick);
                    SetMusicVolume(0.1f);                  
                    break;
            }
        }
    }

    public void LoadSceneAndPlaySound(Sounds sound)
    {
        SoundManager.Instance.Play(sound);
        SceneManager.LoadScene(LevelName);      
    }

    private void SetMusicVolume(float volume)
    {
        SoundManager.Instance.setVolumeOfAudioSource( SoundManager.Instance.soundMusic, volume);
    }
}
