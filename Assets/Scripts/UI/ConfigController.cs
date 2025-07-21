using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigController : MonoBehaviour
{
    [Header("Game Settings UI")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Toggle invertMouseToggle;

    [Header("Audio Settings UI")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    //[Header("Event System")]
    //[SerializeField] private GameObject eventSystem;

    private void Start()
    {
        Debug.Log("Starting ConfigController...");
        // Obtenemos valores desde GameController
        if (GameController.Instance != null)
        {
            mouseSensitivitySlider.value = GameController.Instance.MouseSensitivity;
            //invertMouseToggle.isOn = GameController.Instance.InvertMouse;

            // Agregamos listeners para detectar cambios en UI
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            //invertMouseToggle.onValueChanged.AddListener(OnInvertMouseChanged);

            this.LoadEventSystem();
        }

        // Obtenemos valores desde AudioController
        if (AudioController.Instance != null)
        {
            Debug.Log("AudioController Instance found, setting up audio sliders." +
            AudioController.Instance.MusicVolume + " " + AudioController.Instance.SfxVolume);
            musicVolumeSlider.value = AudioController.Instance.MusicVolume;
            sfxVolumeSlider.value = AudioController.Instance.SfxVolume;

            // Agregamos listeners para detectar cambios en UI
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            Debug.Log("Music Volume: Start  " + AudioController.Instance.MusicVolume);
        }
    }

    private void OnMouseSensitivityChanged(float value)
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.MouseSensitivity = value;
        }
    }

    /*private void OnInvertMouseChanged(bool isInverted)
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.InvertMouse = isInverted;
        }
    }*/

    private void OnMusicVolumeChanged(float value)
    {
        Debug.Log("Changed music volume");
        // Aseguramos que AudioController esté inicializado antes de cambiar el volumen
        // Esto es importante para evitar errores si se accede antes de que AudioController se inicialice
        // o si no existe en la escena actual.
        if (AudioController.Instance != null)
        {
            AudioController.Instance.MusicVolume = value;
        }
    }

    private void OnSfxVolumeChanged(float value)
    {
        if (AudioController.Instance != null)
        {
            AudioController.Instance.SfxVolume = value;
        }
    }

    private void LoadEventSystem()
    {
        if (GameController.Instance.IsPaused())
        {
            //eventSystem.SetActive(false); //Creo que el event system, puede estar siempre desactivado
        }
    }

    public void ReturnToPreviousScene()
    {   
        SceneController.Instance.UnloadConfigAsync();
    }
}
