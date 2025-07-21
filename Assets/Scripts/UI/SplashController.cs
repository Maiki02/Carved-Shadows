using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashController : MonoBehaviour
{
    [SerializeField] private Image splashImage;          // La imagen del splash
    [SerializeField] private Image backgroundImage;      // El objeto UI “Background” en Canvas
    [SerializeField] private float duration = 3f;        // Tiempo total
    [SerializeField] private float postWait = 1f;        // Tiempo de espera después de la animación

    [SerializeField] private string nextSceneName = "MenuScene"; // Nombre de la escena siguiente

    private void Start()
    {
        // Inicializa el fondo UI en gris
        backgroundImage.color = Color.gray;

        StartCoroutine(PlaySplash());
    }

    private IEnumerator PlaySplash()
    {
        float t = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        Color startColor = Color.gray;
        Color endColor = Color.black;

        while (t < duration)
        {
            t += Time.deltaTime;
            float frac = t / duration;

            // Escala de la imagen
            splashImage.rectTransform.localScale = Vector3.Lerp(startScale, endScale, frac);
            // Transición de color del fondo UI
            backgroundImage.color = Color.Lerp(startColor, endColor, frac);

            yield return null;
        }

        // Espera adicional antes de cargar la siguiente escena
        yield return new WaitForSeconds(postWait);

        SceneManager.LoadScene(nextSceneName);
    }
}   
