using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private GameObject menuCamara;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerController.SetControlesActivos(false);
    }

    public void StartGame()
    {
        GameController.Instance.SetGameStarted(true);
        AudioController.Instance.StopMusic();

        // Oculta el menu
        menuUI.SetActive(false);

        // Oculta la cámara del menu
        menuCamara.SetActive(false);

        // Activa controles del jugador
        playerController.SetControlesActivos(true);
    }
}
