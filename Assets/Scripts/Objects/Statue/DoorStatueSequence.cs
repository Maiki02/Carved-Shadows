using System.Collections;
using UnityEngine;

/// <summary>
/// Controla una secuencia completa de puerta-estatua mediante un trigger.
/// Cuando el jugador entra al trigger, cierra la puerta rápidamente y activa la estatua.
/// Incluye configuración automática y manual.
/// </summary>
[RequireComponent(typeof(Collider))]
public class DoorStatueSequence : MonoBehaviour
{
    [Header("Referencias principales")]
    [SerializeField] private Door targetDoor; // La puerta que se cerrará
    [SerializeField] private GameObject targetStatue; // La estatua que aparecerá
    
    [Header("Configuración del trigger")]
    [SerializeField] private bool triggerOnce = true; // Si se activa solo una vez
    
    [Header("Configuración de audio (opcional)")]
    [SerializeField] private AudioClip fastCloseClip; // Clip personalizado para cierre rápido
    
    [Header("Configuración automática")]
    [SerializeField] private bool autoSetupOnStart = true; // Configuración automática al iniciar
    [SerializeField] private bool showDebugLogs = true; // Mostrar logs de debug
    
    private bool hasTriggered = false; // Control para evitar múltiples activaciones
    
    private void Awake()
    {
        ValidateComponents();
    }
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupSequence();
        }
    }
    
    /// <summary>
    /// Valida que todos los componentes necesarios estén presentes
    /// </summary>
    private void ValidateComponents()
    {
        // Asegurar que el collider sea un trigger
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
            if (showDebugLogs)
                Debug.Log($"[DoorStatueSequence] Collider configurado como trigger en {gameObject.name}");
        }
        else
        {
            Debug.LogError($"[DoorStatueSequence] No se encontró Collider en {gameObject.name}. Se requiere para el trigger.");
        }
    }
    
    /// <summary>
    /// Configura automáticamente toda la secuencia
    /// </summary>
    [ContextMenu("Setup Sequence")]
    public void SetupSequence()
    {
        if (showDebugLogs)
            Debug.Log("[DoorStatueSequence] Configurando secuencia...");
        
        // Configurar la puerta
        SetupDoor();
        
        // Configurar la estatua
        SetupStatue();
        
        if (showDebugLogs)
            Debug.Log("[DoorStatueSequence] ¡Secuencia configurada correctamente!");
    }
    
    /// <summary>
    /// Configura la puerta con el audio personalizado si está disponible
    /// </summary>
    private void SetupDoor()
    {
        if (targetDoor != null)
        {
            // Asignar el clip de cierre rápido si está disponible
            if (fastCloseClip != null)
            {
                targetDoor.SetFastCloseClip(fastCloseClip);
                if (showDebugLogs)
                    Debug.Log($"[DoorStatueSequence] Audio clip de cierre rápido asignado a {targetDoor.name}");
            }
            
            if (showDebugLogs)
                Debug.Log($"[DoorStatueSequence] Puerta {targetDoor.name} configurada");
        }
        else
        {
            Debug.LogWarning("[DoorStatueSequence] No se ha asignado ninguna puerta");
        }
    }
    
    /// <summary>
    /// Configura la estatua (la desactiva inicialmente)
    /// </summary>
    private void SetupStatue()
    {
        if (targetStatue != null)
        {
            targetStatue.SetActive(false);
            if (showDebugLogs)
                Debug.Log($"[DoorStatueSequence] Estatua {targetStatue.name} desactivada al inicio");
        }
        else
        {
            Debug.LogWarning("[DoorStatueSequence] No se ha asignado ninguna estatua");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es el jugador y si no se ha activado antes (si triggerOnce está activo)
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && hasTriggered) return;
        
        if (showDebugLogs)
            Debug.Log($"[DoorStatueSequence] Jugador entró al trigger. Activando secuencia...");
        
        // Marcar como activado si es de una sola vez
        if (triggerOnce)
        {
            hasTriggered = true;
        }
        
        // Ejecutar la secuencia
        ActivateSequence();
    }
    
    /// <summary>
    /// Ejecuta la secuencia principal: cierra la puerta rápidamente y activa la estatua
    /// </summary>
    private void ActivateSequence()
    {
        // Cerrar la puerta rápidamente
        if (targetDoor != null)
        {
            if (showDebugLogs)
                Debug.Log($"[DoorStatueSequence] Cerrando puerta {targetDoor.name} rápidamente");
            targetDoor.StartFastClosing();
        }
        
        // Activar la estatua
        if (targetStatue != null)
        {
            if (showDebugLogs)
                Debug.Log($"[DoorStatueSequence] Activando estatua {targetStatue.name}");
            targetStatue.SetActive(true);
        }
    }
    
    /// <summary>
    /// Permite resetear el trigger desde código si es necesario
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        if (showDebugLogs)
            Debug.Log($"[DoorStatueSequence] Trigger reseteado");
    }
    
    /// <summary>
    /// Permite cambiar las referencias desde código
    /// </summary>
    public void SetReferences(Door door, GameObject statue)
    {
        targetDoor = door;
        targetStatue = statue;
        if (showDebugLogs)
            Debug.Log($"[DoorStatueSequence] Referencias actualizadas: Puerta={door?.name}, Estatua={statue?.name}");
    }
    
    /// <summary>
    /// Permite asignar un clip de audio personalizado para el cierre rápido
    /// </summary>
    public void SetFastCloseClip(AudioClip clip)
    {
        fastCloseClip = clip;
        if (targetDoor != null)
        {
            targetDoor.SetFastCloseClip(clip);
        }
    }
    
    private void OnValidate()
    {
        // Validaciones en el editor
        if (targetDoor != null && targetDoor.GetComponent<Door>() == null)
        {
            Debug.LogError("[DoorStatueSequence] El objeto asignado como puerta no tiene componente Door");
        }
        
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("[DoorStatueSequence] El Collider debe estar marcado como 'Is Trigger'");
        }
        else if (col == null)
        {
            Debug.LogWarning("[DoorStatueSequence] Se necesita un Collider configurado como Trigger");
        }
    }
}
