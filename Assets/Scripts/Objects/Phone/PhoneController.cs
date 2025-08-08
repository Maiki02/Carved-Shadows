using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PhoneController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Door door; // Puerta que se cerrará
    [SerializeField] private GameObject telefonoColgado; // Teléfono en estado colgado
    [SerializeField] private GameObject telefonoDescolgado; // Teléfono en estado descolgado
    
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource phoneAudioSource; // AudioSource del teléfono
    [SerializeField] private AudioClip phoneRingClip; // Sonido del teléfono sonando
    [SerializeField] private float ringDuration = 5f; // Duración que suena el teléfono
    
    [Header("Configuración")]
    [SerializeField] private bool triggerOnce = true; // Solo se activa una vez
    
    private bool hasTriggered = false;
    private Coroutine phoneSequenceCoroutine;

    private void Awake()
    {
        // Asegurar que el trigger esté configurado correctamente
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
        
        // Validar referencias
        ValidateReferences();
    }

    private void Start()
    {
        // Asegurar que el teléfono esté en estado inicial (colgado)
        SetPhoneState(true); // true = colgado, false = descolgado
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es el player (asumiendo que tiene tag "Player" o CharacterController)
        if (other.CompareTag("Player") || other.GetComponent<CharacterController>() != null)
        {
            if (triggerOnce && hasTriggered) return; // Si solo se activa una vez y ya se activó
            
            hasTriggered = true;
            StartPhoneSequence();
        }
    }

    /// <summary>
    /// Inicia la secuencia completa: cerrar puerta -> sonar teléfono
    /// </summary>
    public void StartPhoneSequence()
    {
        if (phoneSequenceCoroutine != null)
        {
            StopCoroutine(phoneSequenceCoroutine);
        }
        
        phoneSequenceCoroutine = StartCoroutine(PhoneSequenceCoroutine());
    }

    /// <summary>
    /// Detiene la secuencia del teléfono
    /// </summary>
    public void StopPhoneSequence()
    {
        if (phoneSequenceCoroutine != null)
        {
            StopCoroutine(phoneSequenceCoroutine);
            phoneSequenceCoroutine = null;
        }
        
        StopPhoneRing();
    }

    /// <summary>
    /// Hace que el teléfono suene
    /// </summary>
    public void StartPhoneRing()
    {
        if (phoneAudioSource != null && phoneRingClip != null)
        {
            phoneAudioSource.clip = phoneRingClip;
            phoneAudioSource.loop = true; // Loop para que suene continuamente
            phoneAudioSource.Play();
            Debug.Log("[PhoneController] Teléfono empezó a sonar");
        }
    }

    /// <summary>
    /// Detiene el sonido del teléfono
    /// </summary>
    public void StopPhoneRing()
    {
        if (phoneAudioSource != null)
        {
            phoneAudioSource.Stop();
            Debug.Log("[PhoneController] Teléfono dejó de sonar");
        }
    }

    /// <summary>
    /// Cambia el estado visual del teléfono (colgado/descolgado)
    /// </summary>
    /// <param name="isHanging">true = colgado, false = descolgado</param>
    public void SetPhoneState(bool isHanging)
    {
        if (telefonoColgado != null)
            telefonoColgado.SetActive(isHanging);
        
        if (telefonoDescolgado != null)
            telefonoDescolgado.SetActive(!isHanging);
        
        Debug.Log($"[PhoneController] Teléfono estado: {(isHanging ? "Colgado" : "Descolgado")}");
    }

    /// <summary>
    /// Corrutina principal que maneja toda la secuencia
    /// </summary>
    private IEnumerator PhoneSequenceCoroutine()
    {
        Debug.Log("[PhoneController] Iniciando secuencia del teléfono");
        
        // 1. Activar el cierre de la puerta
        if (door != null)
        {
            Debug.Log("[PhoneController] Cerrando puerta...");
            door.StartSlowClosing(); // Usar el método de cierre lento
            
            // Esperar a que termine el cierre de la puerta
            //yield return new WaitForSeconds(door.SlowCloseDuration);
            Debug.Log("[PhoneController] Puerta cerrada completamente");
        }
        
        // 2. Empezar a sonar el teléfono
        StartPhoneRing();
        
        // 3. Esperar la duración configurada del ring
        yield return new WaitForSeconds(ringDuration);
        
        // 4. Detener el sonido del teléfono
        StopPhoneRing();
        
        // Aquí es donde más adelante se activaría el cambio de cámara/escena
        Debug.Log("[PhoneController] Secuencia completada - Aquí iría el cambio de escena");
    }

    /// <summary>
    /// Valida que todas las referencias estén asignadas
    /// </summary>
    private void ValidateReferences()
    {
        if (door == null)
            Debug.LogWarning("[PhoneController] Puerta no asignada");
        
        if (telefonoColgado == null)
            Debug.LogWarning("[PhoneController] Teléfono colgado no asignado");
        
        if (telefonoDescolgado == null)
            Debug.LogWarning("[PhoneController] Teléfono descolgado no asignado");
        
        if (phoneAudioSource == null)
            Debug.LogWarning("[PhoneController] AudioSource del teléfono no asignado");
        
        if (phoneRingClip == null)
            Debug.LogWarning("[PhoneController] Clip de sonido del teléfono no asignado");
    }

    /// <summary>
    /// Resetea el trigger para que pueda activarse de nuevo
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log("[PhoneController] Trigger reseteado");
    }

    // Para debugging en el editor
    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}