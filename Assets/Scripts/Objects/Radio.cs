using System.Collections;
using UnityEngine;

public class Radio : MonoBehaviour //: ObjectInteract
{
    [SerializeField] private AudioSource audioSource; // Asigna el AudioSource en el Inspector
    [SerializeField] private PlayerController playerController; // Referencia al PlayerController
    [SerializeField] private float radioRange = 5f; // Rango de interacción con la radio
    [SerializeField] private float audioDuration = 121f; // Duración del audio de la radio
    private bool radioReproducida = false; // Bandera para evitar múltiples reproducciones

    [SerializeField] private InventoryHotbar inventarioHotbar;

    [Tooltip("Secuencia de diálogos de la radio")]
    [SerializeField] private DialogMessage[] callDialogSequence;

    public void PlayRadio()
    {
        Debug.Log("Intentando reproducir la radio...");
        StartCoroutine(PlayRadioCoroutine());
    }

    /*public override void OnInteract()
    {
        if (inventarioHotbar != null && inventarioHotbar.TieneObjectoSeleccionado("knob"))
        {
            inventarioHotbar.RemoveSelectedPiece(); // Quitamos la pieza del inventario
            PlayRadio();
            this.radioReproducida = true; // Marcamos que la radio ha sido reproducida
        }
        else
        {
            if(this.radioReproducida) return; 
            DialogController.Instance.ShowDialog("Le falta una perilla.", 2f);
        }
    }**/

    public IEnumerator PlayRadioCoroutine()
    {
        audioSource.Play();

        playerController.SetControlesActivos(false); // Desactiva los controles del jugador

        // Inicia la secuencia de diálogos en paralelo
        Coroutine dialogCoroutine = null;
        if (callDialogSequence != null && callDialogSequence.Length > 0)
        {
            dialogCoroutine = StartCoroutine(ShowDialogSequenceParallel());
        }

        yield return new WaitForSeconds(audioDuration); // Espera la duración del audio

        // Si la secuencia de diálogos sigue, la detenemos (opcional, depende del comportamiento deseado)
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }

        playerController.SetControlesActivos(true); // Reactiva los controles del jugador

        //StartCoroutine(FadeManager.Instance.Oscurecer());

        //SceneController.Instance.LoadGameOverScene();
    }

    // Corrutina para mostrar la secuencia de diálogos en paralelo
    private IEnumerator ShowDialogSequenceParallel()
    {
        DialogController.Instance.ShowDialogSequence(callDialogSequence);
        // Calcula la duración total de los diálogos
        float totalDuration = 0f;
        foreach (var msg in callDialogSequence)
            totalDuration += msg.duration;
        yield return new WaitForSeconds(totalDuration);
    }
}
