using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteController : MonoBehaviour
{
    [SerializeField] private Button loadNextLevelButton;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        loadNextLevelButton.onClick.AddListener(LoadNextLevel);
    }

    private void Start()
    {        
        if(LevelManager.Instance.CheckForLastLevel())
        {
            loadNextLevelButton.gameObject.SetActive(false);
            Vector2 buttonPosition = loadNextLevelButton.GetComponent<RectTransform>().anchoredPosition;
            backButton.GetComponent<RectTransform>().anchoredPosition = buttonPosition;
        }       
    }

    public void LevelComplete()
    {
        gameObject.SetActive(true);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
