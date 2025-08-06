using UnityEngine;
using System.Collections;

public class MenuInitializer : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;   // Objeto MainMenu completo
    [SerializeField] private GameObject[] canvasToActivate; // Otros Canvas que se activan al iniciar el juego
    [SerializeField] private GameObject menuCamera; // Cámara del menú


    private PlayerController playerController;

    IEnumerator Start()
    {
        var playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            playerController = playerGO.GetComponent<PlayerController>();

        if (menuRoot != null) menuRoot.SetActive(false);
        //if (playerUI != null) playerUI.SetActive(false);
        if (menuCamera != null) menuCamera.SetActive(true);

        if (playerController != null)
            playerController.SetControlesActivos(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return null;

        if (menuRoot != null)
            menuRoot.SetActive(true);
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        GameController.Instance.SetGameStarted(true);
        AudioController.Instance.StopMusic();

        if (menuRoot != null) menuRoot.SetActive(false);

        if (menuCamera != null) menuCamera.SetActive(false);

        //Activamos la UI del jugador antes de la animación
        this.ShowCanvasToActivate();

        // 2 segs para la transicion de cam prolija
        yield return new WaitForSeconds(2f);


        if (playerController != null)
            playerController.SetControlesActivos(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowCanvasToActivate()
    {
        if (canvasToActivate != null)
        {
            foreach (var canvas in canvasToActivate)
            {
                if (canvas != null)
                {
                    canvas.SetActive(true);
                }
            }
        }
    }
}
