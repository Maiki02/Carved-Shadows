using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Start()
    {
        AudioController.Instance.PlayMusic(AudioType.BackgroundMusicMenu, true);
        Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
        Cursor.visible = true; // Hace visible el cursor
    }

    public void StartGame()
    {
        GameController.Instance.StartGame();
        AudioController.Instance.StopMusic();
    }

    public void HowToPlay()
    {
        SceneController.Instance.LoadHowToPlayScene();
    }

    public void Configuration()
    {
        SceneController.Instance.LoadConfigurationSceneWithAdditive();
    }

    public void QuitGame()
    {
        SceneController.Instance.QuitGame();
    }
}