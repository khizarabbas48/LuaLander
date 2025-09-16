using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Time.timeScale = 1f; // Ensure time scale is normal when in main menu
        playButton.onClick.AddListener(() =>
        {
            // Code to start the game
            GameManager.ResetStaticData();
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            // Code to quit the game
            Debug.Log("Quit button clicked");
            Application.Quit();
        });
    }
    
    private void Start()
    {
        playButton.Select();
    }
}
