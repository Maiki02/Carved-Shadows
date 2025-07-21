using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteract : MonoBehaviour, IInteractable
{
    private Outline _outline;  // O el componente que uses para hacer el contorno

    protected virtual void Awake()
    {

        // Asegúrate de que haya un Collider
        if (GetComponent<Collider>() == null)
        {
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                var mc = gameObject.AddComponent<MeshCollider>();
                mc.convex = true; // para que actúe como trigger o con Rigidbody
            }
        }

        // Busca o añade tu componente de outline
        _outline = GetComponent<Outline>();
        if (_outline == null)
        {
            _outline = gameObject.AddComponent<Outline>();
            // configura aquí tu material, ancho de línea, etc.
        }
        _outline.enabled = false;

    }

    public virtual void OnHoverEnter()
    {
        if(_outline == null) return;
        _outline.enabled = true;
    }

    public virtual void OnHoverExit()
    {
        if (_outline == null) return;
        _outline.enabled = false;
    }

    public virtual void OnInteract()
    {
        Debug.Log($"Interacción con {gameObject.name}");
    }
}