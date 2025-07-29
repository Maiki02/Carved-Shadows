
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door: ObjectInteract
{
    [Header("Knocking Loop")]
    [SerializeField] private float knockingInterval = 3f; // Intervalo entre golpes en segundos
    private Coroutine knockingLoopCoroutine;
    
    [Header("Requisitos")]
    [SerializeField] private PuzzlePiece objetoRequerido;
    [SerializeField] private InventoryHotbar inventarioHotbar;

    [Header("Tipo de acción sobre la puerta")]
    [SerializeField] private TypeDoorInteract type = TypeDoorInteract.None;

    private bool isDoorOpen = false;
    [Header("Configuración de apertura por rotación")]
    [SerializeField] private float openDegreesY = 110f; // Grados de apertura en Y
    [SerializeField] private float openDuration = 1f; // Duración de apertura en segundos
    [SerializeField] private float knockAmount = 15f; // Grados del golpe
    [SerializeField] private float knockSpeed = 8f; // Velocidad del golpe
    private Quaternion initialRotation;
    private Coroutine doorCoroutine;

    protected override void Awake()
    {
        base.Awake(); // Llamamos al Awake de la clase base para inicializar el objeto interactivo
        initialRotation = transform.rotation;
    }

    private void Start()
    {
        this.StartKnockingLoop();   
    }

    private void Update()
    {
    }

    public override void OnHoverEnter()
    {
        if (type == TypeDoorInteract.None) return; // Si no es del tipo None, no hacemos nada
        
        base.OnHoverEnter(); // Llamamos al método base para activar el contorno

    }

    public override void OnHoverExit()
    {
        if (type == TypeDoorInteract.None) return; // Si no es del tipo None, no hacemos nada
        
        base.OnHoverExit(); // Llamamos al método base para desactivar el contorno

    }

    public override void OnInteract()
    {

        isDoorOpen = !isDoorOpen; // Cambiamos el estado de la puerta

        this.ValidateDoorWithAnimation();
        this.ValidateDoorWithTeleport();
        this.ValidateDoorWithNextLevel();
    }
    
    public void SetType(TypeDoorInteract newType)
    {
        type = newType;
    }

    /// <summary>
    /// Abre la puerta rotando en Y según el atributo openDegreesY
    /// </summary>
    public void OpenDoorByRotation()
    {
        if (doorCoroutine != null) StopCoroutine(doorCoroutine);
        doorCoroutine = StartCoroutine(RotateDoorCoroutine(openDegreesY));
    }

    /// <summary>
    /// Simula que la puerta "late" (golpe sutil y regresa a rotación original)
    /// </summary>
    public void KnockDoor()
    {
        if (doorCoroutine != null) StopCoroutine(doorCoroutine);
        doorCoroutine = StartCoroutine(KnockDoorCoroutine());
    }

    /// <summary>
    /// Inicia el bucle de golpeo (Knocking) si el tipo es Knocking
    /// </summary>
    public void StartKnockingLoop()
    {
        if (type != TypeDoorInteract.Knocking) return;
        if (knockingLoopCoroutine != null) StopCoroutine(knockingLoopCoroutine);
        knockingLoopCoroutine = StartCoroutine(KnockingLoopCoroutine());
        Debug.Log("[Door] Iniciando bucle de Knocking");
    }

    /// <summary>
    /// Detiene el bucle de golpeo (Knocking)
    /// </summary>
    public void StopKnockingLoop()
    {
        if (knockingLoopCoroutine != null)
        {
            StopCoroutine(knockingLoopCoroutine);
            knockingLoopCoroutine = null;
            Debug.Log("[Door] Deteniendo bucle de Knocking");
        }
    }

    private IEnumerator KnockingLoopCoroutine()
    {
        while (true)
        {
            KnockDoor();
            yield return new WaitForSeconds(knockingInterval);
        }
    }

    private IEnumerator RotateDoorCoroutine(float targetDegrees)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y + targetDegrees, startRot.eulerAngles.z);
        float elapsed = 0f;
        while (elapsed < openDuration)
        {
            float t = Mathf.Clamp01(elapsed / openDuration);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRot;
    }

    private IEnumerator KnockDoorCoroutine()
    {
        Quaternion startRot = initialRotation;
        Quaternion knockRot = Quaternion.Euler(startRot.eulerAngles.x, startRot.eulerAngles.y + knockAmount, startRot.eulerAngles.z);
        float t = 0f;
        // Golpe hacia knockAmount
        while (t < 1f)
        {
            t += Time.deltaTime * knockSpeed;
            transform.rotation = Quaternion.Slerp(startRot, knockRot, t);
            yield return null;
        }
        transform.rotation = knockRot;
        // Regresa a rotación original
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * knockSpeed;
            transform.rotation = Quaternion.Slerp(knockRot, startRot, t);
            yield return null;
        }
        transform.rotation = startRot;
    }
    // Elimino llave de cierre extra para que las funciones siguientes estén dentro de la clase

    private void ValidateDoorWithAnimation()
    {
        if (type != TypeDoorInteract.OpenAndClose) return; // Si no es del tipo OpenAndClose, no hacemos nada

        this.OpenOrCloseDoor(isDoorOpen);


    }

    private void ValidateDoorWithNextLevel()
    {
        if (type != TypeDoorInteract.NextLevel) return; // Si no es del tipo NextLevel, no hacemos nada
        isDoorOpen = true; // Forzamos la apertura de la puerta para el siguiente nivel

        if (isDoorOpen)
        {
            GameFlowManager.Instance.GoToNextLevel(); // Subimos el nivel, (para activar otra room)
        }
        else
        {
            DialogController.Instance.ShowDialog("La puerta está cerrada. Necesitas abrirla para continuar.", 2f);
        }
    }

    public void OpenOrCloseDoor(bool isDoorOpen)
    {
        // Ahora abrimos/cerramos con rotación en vez de animación
        if (isDoorOpen)
        {
            Debug.Log($"[Door] Abriendo puerta: rotando a {openDegreesY} grados Y");
            if (doorCoroutine != null) StopCoroutine(doorCoroutine);
            doorCoroutine = StartCoroutine(RotateDoorCoroutine(openDegreesY));
            AudioController.Instance.PlaySFX(AudioType.DoorOpen);
        }
        else
        {
            Debug.Log($"[Door] Cerrando puerta: rotando a {-openDegreesY} grados Y (posición original)");
            if (doorCoroutine != null) StopCoroutine(doorCoroutine);
            doorCoroutine = StartCoroutine(RotateDoorCoroutine(-openDegreesY));
            AudioController.Instance.PlaySFX(AudioType.DoorClose);
        }

    }

    private void ValidateDoorWithTeleport()
    {
        if (type != TypeDoorInteract.Key) { return; } //Si no es del tipo Key, no hacemos nada

        //Debug.Log("Tiene key: " + TieneObjetoEnInventario());

        if (inventarioHotbar != null && inventarioHotbar.TieneObjetoSeleccionado(objetoRequerido))
        {

            isDoorOpen = true;
            inventarioHotbar.RemoveSelectedPiece(); // Quitamos la pieza del inventario
            GameFlowManager.Instance.GoToNextLevel(); // Subimos el nivel, (para activar otra room)
        }
        else
        {
            DialogController.Instance.ShowDialog("La puerta está cerrada. Necesitas una llave.", 2f);
        }
    }

    /*private bool TieneObjetoEnInventario()
    {
        if (inventarioHotbar == null || objetoRequerido == null)
            return false;

        PuzzlePiece[] piezas = inventarioHotbar.ObtenerPiezas();
        foreach (PuzzlePiece pieza in piezas)
        {
            if (pieza != null && pieza == objetoRequerido)
            {
                return true;
            }
        }
        return false;
    }*/

}
