using UnityEngine;
using UnityEngine.UI;


public class InventoryHotbar : MonoBehaviour
{
    public Image[] slotImages;
    private PuzzlePiece[] pieces = new PuzzlePiece[5];
    private int selectedSlot = -1;

    void Update()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                SelectPiece(i);
            }
        }
    }

    void SelectPiece(int index)
    {
        // Si ya está seleccionado, deseleccionamos
        if (selectedSlot == index)
        {
            var currentImage = slotImages[selectedSlot];
            if (currentImage != null)
            {
                var existingOutline = currentImage.GetComponent<UnityEngine.UI.Outline>();
                if (existingOutline != null)
                {
                    Destroy(existingOutline);
                }
            }

            selectedSlot = -1;
            Debug.Log("Slot deseleccionado.");
            return;
        }

        if (pieces[index] == null) return;

        // Quitar outline de todos los slots
        for (int i = 0; i < slotImages.Length; i++)
        {
            var oldOutline = slotImages[i].GetComponent<UnityEngine.UI.Outline>();
            if (oldOutline != null)
            {
                Destroy(oldOutline);
            }
        }

        // Agregar outline al nuevo slot seleccionado
        var image = slotImages[index];
        if (image != null)
        {
            var outline = image.gameObject.AddComponent<UnityEngine.UI.Outline>();
            outline.effectColor = Color.red;
            outline.effectDistance = new Vector2(5f, -5f);
            outline.useGraphicAlpha = true;
        }

        selectedSlot = index;
        Debug.Log("Seleccionaste pieza en slot: " + (index + 1));
    }

    public void AddPiece(PuzzlePiece piece)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] == null)
            {
                pieces[i] = piece;
                slotImages[i].sprite = piece.GetIconUI();
                slotImages[i].color = new Color(1f, 1f, 1f, 1f); //Aclaramos para que se vea la imagen
                slotImages[i].enabled = true;
                piece.gameObject.SetActive(false);
                Debug.Log($"Pieza {piece.name} agregada al slot {i + 1}");
                return;
            }
        }

        Debug.Log("Inventario lleno, no se puede agregar la pieza.");
    }

    public PuzzlePiece GetSelectedPiece()
    {
        if (selectedSlot >= 0 && selectedSlot < pieces.Length)
        {
            return pieces[selectedSlot];
        }

        return null;
    }

    public void RemoveSelectedPiece()
    {
        if (selectedSlot >= 0 && selectedSlot < pieces.Length)
        {
            // Eliminar el outline antes de resetear el índice
            var outline = slotImages[selectedSlot].GetComponent<UnityEngine.UI.Outline>();
            if (outline != null)
            {
                Destroy(outline);
            }

            slotImages[selectedSlot].sprite = null;
            slotImages[selectedSlot].enabled = true;
            slotImages[selectedSlot].color = new Color(0f, 0f, 0f, 0.54f);
            pieces[selectedSlot] = null;
            selectedSlot = -1;
        }
        else
        {
            Debug.LogWarning("No hay slot seleccionado o índice fuera de rango.");
        }
    }


    public void ResetSelection()
    {
        selectedSlot = -1;

        // Eliminar todos los outlines
        foreach (var img in slotImages)
        {
            var outline = img.GetComponent<Outline>();
            if (outline != null) Destroy(outline);
        }
    }

    public bool TieneObjeto(string nombreObjeto)
    {
        foreach (var pieza in pieces)
        {
            if (pieza != null && pieza.name == nombreObjeto)
            {
                return true;
            }
        }
        return false;
    }

    public bool TieneObjetoSeleccionado(PuzzlePiece piezaSeleccionada)
    {
        if (selectedSlot < 0 || selectedSlot >= pieces.Length)
            return false;

        return pieces[selectedSlot] == piezaSeleccionada;
    }

    public bool TieneObjectoSeleccionado(string nombreObjeto)
    {
        if (selectedSlot < 0 || selectedSlot >= pieces.Length)
            return false;

        return pieces[selectedSlot] != null && pieces[selectedSlot].name == nombreObjeto;
    }

    public PuzzlePiece[] ObtenerPiezas()
    {
        return pieces;
    }
}
