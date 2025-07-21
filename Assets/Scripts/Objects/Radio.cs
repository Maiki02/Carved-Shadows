using System.Collections;
using UnityEngine;

public class Radio : ObjectInteract
{
    [SerializeField] private AudioSource audioSource; // Asigna el AudioSource en el Inspector
    [SerializeField] private PlayerController playerController; // Referencia al PlayerController
    [SerializeField] private float radioRange = 5f; // Rango de interacción con la radio
    [SerializeField] private float audioDuration = 121f; // Duración del audio de la radio
    private bool radioReproducida = false; // Bandera para evitar múltiples reproducciones

    [SerializeField] private InventoryHotbar inventarioHotbar;

    public void PlayRadio()
    {
        Debug.Log("Intentando reproducir la radio...");
        StartCoroutine(PlayRadioCoroutine());
    }

    public override void OnInteract()
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
    }

    public IEnumerator PlayRadioCoroutine()
    {
        audioSource.Play();

        playerController.SetControlesActivos(false); // Desactiva los controles del jugador

        yield return new WaitForSeconds(audioDuration); // Espera la duración del audio

        playerController.SetControlesActivos(true); // Reactiva los controles del jugador

        StartCoroutine(FadeManager.Instance.Oscurecer());

        SceneController.Instance.LoadGameOverScene();
    }
}
