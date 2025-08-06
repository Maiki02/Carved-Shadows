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

    [SerializeField] private string nextSceneName = "LoadingScene"; // Nombre de la escena siguiente

    private void Start()
    {
        // Inicializa el fondo UI en negro y completamente transparente
        backgroundImage.color = new Color(0f, 0f, 0f, 0f);
        StartCoroutine(FadeInBlack());
    }

    private IEnumerator FadeInBlack()
    {
        float t = 0f;
        Color startColor = new Color(0f, 0f, 0f, 0f); // negro transparente
        Color endColor = new Color(0f, 0f, 0f, 1f);   // negro opaco

        // Si splashImage existe, asegúrate de que esté visible (pero no se animará)
        if (splashImage != null)
            splashImage.enabled = true;

        while (t < duration)
        {
            t += Time.deltaTime;
            float frac = t / duration;
            backgroundImage.color = Color.Lerp(startColor, endColor, frac);
            yield return null;
        }

        backgroundImage.color = endColor;

        // Espera adicional antes de cargar la siguiente escena
        yield return new WaitForSeconds(postWait);

        SceneManager.LoadScene(nextSceneName);
    }
}   
