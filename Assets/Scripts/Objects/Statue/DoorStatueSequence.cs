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
    
    #region Gizmos
    
    private void OnDrawGizmos()
    {
        DrawTriggerGizmos(false);
    }
    
    private void OnDrawGizmosSelected()
    {
        DrawTriggerGizmos(true);
    }
    
    /// <summary>
    /// Dibuja los gizmos del trigger para visualización en el editor
    /// </summary>
    /// <param name="isSelected">Si el objeto está seleccionado</param>
    private void DrawTriggerGizmos(bool isSelected)
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;
        
        // Colores diferentes según el estado
        Color gizmoColor;
        if (!col.isTrigger)
        {
            gizmoColor = Color.red; // Rojo si no es trigger (error)
        }
        else if (hasTriggered && Application.isPlaying)
        {
            gizmoColor = Color.gray; // Gris si ya se activó
        }
        else if (isSelected)
        {
            gizmoColor = Color.yellow; // Amarillo cuando está seleccionado
        }
        else
        {
            gizmoColor = Color.green; // Verde normal
        }
        
        // Ajustar transparencia
        gizmoColor.a = isSelected ? 0.6f : 0.3f;
        Gizmos.color = gizmoColor;
        
        // Dibujar según el tipo de collider
        if (col is BoxCollider boxCol)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawCube(boxCol.center, boxCol.size);
            
            // Wireframe siempre visible
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireCube(boxCol.center, boxCol.size);
        }
        else if (col is SphereCollider sphereCol)
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position + sphereCol.center, transform.rotation, transform.lossyScale);
            Gizmos.DrawSphere(Vector3.zero, sphereCol.radius);
            
            // Wireframe siempre visible
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireSphere(Vector3.zero, sphereCol.radius);
        }
        else if (col is CapsuleCollider capsuleCol)
        {
            // Para capsule, dibujaremos una esfera aproximada
            Gizmos.matrix = Matrix4x4.TRS(transform.position + capsuleCol.center, transform.rotation, transform.lossyScale);
            float radius = Mathf.Max(capsuleCol.radius, capsuleCol.height * 0.5f);
            Gizmos.DrawSphere(Vector3.zero, radius);
            
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
            Gizmos.DrawWireSphere(Vector3.zero, radius);
        }
        
        // Resetear matrix
        Gizmos.matrix = Matrix4x4.identity;
        
        // Dibujar conexiones con referencias (solo cuando está seleccionado)
        if (isSelected)
        {
            DrawConnectionGizmos();
        }
    }
    
    /// <summary>
    /// Dibuja líneas conectando el trigger con la puerta y estatua
    /// </summary>
    private void DrawConnectionGizmos()
    {
        Vector3 triggerPos = transform.position;
        
        // Conexión con la puerta
        if (targetDoor != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(triggerPos, targetDoor.transform.position);
            
            // Dibujar un pequeño icono en la puerta
            Gizmos.DrawWireCube(targetDoor.transform.position + Vector3.up * 2f, Vector3.one * 0.5f);
        }
        
        // Conexión con la estatua
        if (targetStatue != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(triggerPos, targetStatue.transform.position);
            
            // Dibujar un pequeño icono en la estatua
            Gizmos.DrawWireSphere(targetStatue.transform.position + Vector3.up * 2f, 0.3f);
        }
    }
    
    #endregion
    
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
