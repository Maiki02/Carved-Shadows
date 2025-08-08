using System.Collections;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(AudioSource))]
public class PhoneOpen : MonoBehaviour
{
    private PlayerController playerController;
    
    [Header("Audio Configuration")]
    [SerializeField] private AudioClip callClip; // Clip de la llamada/conversación
    [SerializeField] private AudioClip hangupClip; // Clip de colgar la llamada
    
    [Header("Phone Call Settings")]
    [Tooltip("Secuencia de diálogos de la llamada telefónica")]
    [SerializeField] private DialogMessage[] callDialogSequence;
    
    [Header("Camera Settings")]
    [Tooltip("Priority to set for the phone camera during the call.")]
    public int phoneCameraPriority = 20;
    
    [Header("Controller Reference")]
    [SerializeField] private PhoneController phoneController; // Referencia al controlador principal
    
    private CinemachineVirtualCamera phoneCamera;
    private AudioSource audioSource;
    private bool isCalling = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Busca la cámara virtual con el tag "CameraPhone" en los hijos
        foreach (var cam in GetComponentsInChildren<CinemachineVirtualCamera>(true))
        {
            if (cam.CompareTag("CameraPhone"))
            {
                phoneCamera = cam;
                break;
            }
        }
        if (phoneCamera == null)
        {
            Debug.LogError($"No CinemachineVirtualCamera with tag 'CameraPhone' found in {gameObject.name}.");
        }

        // Busca el PlayerController por tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController component not found on Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }
    }

    private void Start()
    {
        ValidateReferences();
        
        // Este GameObject debería estar desactivado al inicio
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Inicia la llamada telefónica
    /// </summary>
    public void StartCall()
    {
        if (isCalling) return;
        
        Debug.Log("[PhoneOpen] Iniciando llamada...");
        StartCoroutine(PhoneCallRoutine());
    }

    /// <summary>
    /// Corrutina principal que maneja la llamada
    /// </summary>
    private IEnumerator PhoneCallRoutine()
    {
        if (isCalling) yield break;

        isCalling = true;
        
        // Desactiva controles del Player
        if (playerController != null)
            playerController.SetControlesActivos(false);

        // Sube la prioridad para activar la cámara del teléfono
        if (phoneCamera != null)
            phoneCamera.Priority = phoneCameraPriority;

        // Reproducir sonido de la llamada si existe
        if (audioSource != null && callClip != null)
        {
            audioSource.clip = callClip;
            audioSource.loop = false;
            audioSource.Play();
        }

        // Esperar un poco antes de empezar los diálogos
        yield return new WaitForSeconds(0.5f);

        // Muestra la secuencia de diálogos y espera a que termine
        if (callDialogSequence != null && callDialogSequence.Length > 0)
        {
            yield return StartCoroutine(ShowDialogSequenceAndWait());
        }

        // Termina la llamada
        EndCall();
    }

    /// <summary>
    /// Termina la llamada y restaura el estado normal
    /// </summary>
    private void EndCall()
    {
        Debug.Log("[PhoneOpen] Terminando llamada...");
        
        // Reproducir sonido de colgar
        if (audioSource != null && hangupClip != null)
        {
            audioSource.clip = hangupClip;
            audioSource.loop = false;
            audioSource.Play();
        }
        
        StartCoroutine(EndCallRoutine());
    }

    /// <summary>
    /// Corrutina que maneja el final de la llamada
    /// </summary>
    private IEnumerator EndCallRoutine()
    {
        // Esperar a que termine el sonido de colgar
        if (hangupClip != null)
        {
            yield return new WaitForSeconds(hangupClip.length);
        }
        
        // TODO: Implementar fade out aquí
        Debug.Log("[PhoneOpen] Iniciando fade out final...");
        
        yield return new WaitForSeconds(0.5f);
        
        // Baja la prioridad de la cámara del teléfono
        if (phoneCamera != null)
            phoneCamera.Priority = 0;
        
        // Reactiva controles del Player
        if (playerController != null)
            playerController.SetControlesActivos(true);
        
        // Notificar al controller que la llamada terminó
        if (phoneController != null)
        {
            phoneController.OnCallCompleted();
        }
        
        // TODO: Volver a la escena original aquí
        Debug.Log("[PhoneOpen] Volviendo a escena original...");
        
        isCalling = false;
        
        // Desactivar este GameObject
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Corrutina auxiliar para esperar a que termine la secuencia de diálogos
    /// </summary>
    private IEnumerator ShowDialogSequenceAndWait()
    {
        bool finished = false;
        yield return StartCoroutine(DialogSequenceCoroutine(() => finished = true));
        while (!finished) yield return null;
    }

    /// <summary>
    /// Corrutina que ejecuta la secuencia y llama al callback al terminar
    /// </summary>
    private IEnumerator DialogSequenceCoroutine(System.Action onComplete)
    {
        DialogController.Instance.ShowDialogSequence(callDialogSequence);
        float totalDuration = 0f;
        foreach (var msg in callDialogSequence)
            totalDuration += msg.duration;
        yield return new WaitForSeconds(totalDuration);
        onComplete?.Invoke();
    }

    /// <summary>
    /// Valida las referencias necesarias
    /// </summary>
    private void ValidateReferences()
    {
        if (callClip == null)
            Debug.LogWarning("[PhoneOpen] Call clip no asignado");
        
        if (hangupClip == null)
            Debug.LogWarning("[PhoneOpen] Hangup clip no asignado");
        
        if (callDialogSequence == null || callDialogSequence.Length == 0)
            Debug.LogWarning("[PhoneOpen] Secuencia de diálogos no asignada");
        
        if (phoneController == null)
            Debug.LogWarning("[PhoneOpen] PhoneController no asignado");
        
        if (phoneCamera == null)
            Debug.LogWarning("[PhoneOpen] Phone camera no encontrada");
    }
}