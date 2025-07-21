using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }
    [SerializeField] private GameObject pauseMenuUI;

    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) this.TogglePauseMenu();
    }

    public void SetShowPauseUI(bool show)
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(show);
        }
    }

    public void ResumeGame()
    {
        this.TogglePauseMenu();
    }

    public void OpenConfiguration()
    {
        //this.TogglePauseMenu();
        SceneController.Instance.LoadConfigurationSceneWithAdditive();

        //Cuando se abra la configuración, el juego sigue en pausa

    }

    public void ResetGame()
    {
        this.TogglePauseMenu();
        GameController.Instance.ResetGame(true);
    }

    public void ExitToMainMenu()
    {
        this.TogglePauseMenu();
        GameController.Instance.SetGameStarted(false);

        SceneController.Instance.LoadMenuScene();
    }

    public void TogglePauseMenu()
    {
        if (!GameController.Instance.IsGameStarted()) return;

        bool isPaused = !GameController.Instance.IsPaused(); //Guardamos el valor para reutilizarlo

        GameController.Instance.SetIsPaused(isPaused);
        SetShowPauseUI(isPaused);

        if (isPaused) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f; // Asegurarse de que el tiempo se detenga
        } else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f; // Asegurarse de que el tiempo se reanude
        }
        
        
    }
}
