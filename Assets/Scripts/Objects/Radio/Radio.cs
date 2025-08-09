using System.Collections;
using UnityEngine;

/// <summary>
/// Script simplificado de Radio que reproduce audio y diálogos.
/// Ahora funciona principalmente como ejecutor, recibiendo parámetros desde RadioController.
/// Los parámetros como clips, duración y diálogos se manejan desde el controller.
/// </summary>
public class Radio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Asigna el AudioSource en el Inspector
    private bool radioReproducida = false; // Bandera para evitar múltiples reproducciones

    private void Start()
    {
        ValidateReferences();
    }

    /// <summary>
    /// Reproduce la radio con los parámetros proporcionados por el controller
    /// </summary>
    /// <param name="audioClip">Clip de audio a reproducir</param>
    /// <param name="audioDuration">Duración del audio</param>
    /// <param name="dialogSequence">Secuencia de diálogos</param>
    public void PlayRadioWithParameters(AudioClip audioClip, float audioDuration, DialogMessage[] dialogSequence)
    {
        if (radioReproducida)
        {
            Debug.Log("[Radio] La radio ya fue reproducida anteriormente.");
            return;
        }

        Debug.Log("[Radio] Iniciando reproducción de radio...");
        StartCoroutine(PlayRadioCoroutine(audioClip, audioDuration, dialogSequence));
    }

    /// <summary>
    /// Método simplificado para reproducir la radio (mantiene compatibilidad)
    /// </summary>
    public void PlayRadio()
    {
        Debug.Log("[Radio] PlayRadio() llamado sin parámetros - usar PlayRadio con parámetros desde RadioController");
    }

    private IEnumerator PlayRadioCoroutine(AudioClip audioClip, float audioDuration, DialogMessage[] dialogSequence)
    {
        // Configurar y reproducir el audio
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("[Radio] No se proporcionó AudioClip, reproduciendo clip actual del AudioSource");
            //audioSource.Play();
        
        }

        radioReproducida = true;

        // Inicia la secuencia de diálogos en paralelo
        Coroutine dialogCoroutine = null;
        if (dialogSequence != null && dialogSequence.Length > 0)
        {
            dialogCoroutine = StartCoroutine(ShowDialogSequenceParallel(dialogSequence));
        }

        // Esperar la duración del audio
        yield return new WaitForSeconds(audioDuration);

        // Si la secuencia de diálogos sigue, la detenemos (opcional)
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }

        Debug.Log("[Radio] Reproducción de radio completada.");

        // TODO: Agregar aquí lógica adicional post-reproducción si es necesaria
        // StartCoroutine(FadeManager.Instance.Oscurecer());
        // SceneController.Instance.LoadGameOverScene();
    }

    // Corrutina para mostrar la secuencia de diálogos en paralelo
    private IEnumerator ShowDialogSequenceParallel(DialogMessage[] dialogSequence)
    {
        Debug.Log("[Radio] Iniciando secuencia de diálogos paralela. " + dialogSequence.Length);
        DialogController.Instance.ShowDialogSequence(dialogSequence);
        // Calcula la duración total de los diálogos
        float totalDuration = 0f;
        foreach (var msg in dialogSequence)
            totalDuration += msg.duration;
        yield return new WaitForSeconds(totalDuration);
    }

    /// <summary>
    /// Resetea el estado de la radio (para testing)
    /// </summary>
    public void ResetRadio()
    {
        radioReproducida = false;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        Debug.Log("[Radio] Radio reseteada.");
    }

    /// <summary>
    /// Valida que las referencias necesarias estén asignadas
    /// </summary>
    private void ValidateReferences()
    {
        if (audioSource == null)
        {
            Debug.LogError($"[Radio] {name}: Falta asignar AudioSource");
        }
    }

    /// <summary>
    /// Getter para verificar si la radio ya fue reproducida
    /// </summary>
    public bool IsRadioPlayed => radioReproducida;
}
