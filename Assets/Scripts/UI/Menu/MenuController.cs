using UnityEngine;

public class MenuController : MonoBehaviour
{
    private MenuInitializer initializer;

    private void Awake()
    {
        initializer = FindObjectOfType<MenuInitializer>();
    }

    public void OnStartButton()
    {
        initializer?.StartGame();
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
