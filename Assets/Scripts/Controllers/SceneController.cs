using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadIntroScene()
    {
        // Carga la escena de introducción
        LoadScene("IntroScene");
    }

    //La pasamos por parametro para testear distintas.
    public void LoadGameScene(string sceneGame)
    {
        LoadScene(sceneGame);
    }

    public void LoadGameOverScene()
    {
        LoadScene("GameOverScene");
    }

    public void LoadHowToPlayScene()
    {
        LoadScene("TutorialScene");
    }

    /*public void LoadConfigurationScene()
    {
        LoadScene("ConfigScene");
    }*/

    public void LoadConfigurationSceneWithAdditive()
    {
        // Carga la escena de configuración de forma aditiva
        SceneManager.LoadScene("ConfigScene", LoadSceneMode.Additive);
    }

    public void UnloadConfigAsync()
    {
        // Descarga la escena de configuración de forma asíncrona
        SceneManager.UnloadSceneAsync("ConfigScene").completed += _ =>
{
    // Encuentra y reactiva tu EventSystem principal
    var es = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>(true);
    if (es) es.gameObject.SetActive(true);

    var main = SceneManager.GetSceneByName("Main");
    if (main.isLoaded)
        SceneManager.SetActiveScene(main);
};
    }

    public void LoadMenuScene()
    {
        LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo...");
        Application.Quit();
    }



    public void LoadScene(string sceneName)
    {
        //previusSceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(sceneName);

    }

}
