
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door: ObjectInteract
{
    
    [Header("Requisitos")]
    [SerializeField] private PuzzlePiece objetoRequerido;
    [SerializeField] private InventoryHotbar inventarioHotbar;

    [Header("Configuración de la animaación de la puerta")]
    [SerializeField] private Animator doorAnimator;

    [Header("Tipo de acción sobre la puerta")]
    [SerializeField] private TypeDoorInteract type = TypeDoorInteract.None;


    private bool isDoorOpen = false;

    protected override void Awake()
    {
        base.Awake(); // Llamamos al Awake de la clase base para inicializar el objeto interactivo
    }

    private void Update()
    {
        // Si el jugador está cerca y aún no abrimos, al pulsar E ejecutamos la acción:
        
        /*if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            isDoorOpen = !isDoorOpen; // Cambiamos el estado de la puerta

            this.ValidateDoorWithAnimation();

            this.ValidateDoorWithTeleport();

            //AudioController.Instance.PlaySFX(AudioType.DoorOpen);
        }*/
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
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", isDoorOpen); // Activamos la animación de abrir/cerrar

            //yield return new WaitForSeconds(0.1f); // Esperamos un poco para que el sonido se reproduzca
            AudioController.Instance.PlaySFX(isDoorOpen ? AudioType.DoorOpen : AudioType.DoorClose);

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
