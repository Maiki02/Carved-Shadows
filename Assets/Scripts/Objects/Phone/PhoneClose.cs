using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PhoneClose : ObjectInteract
{
    [Header("Audio Configuration")]
    [SerializeField] private AudioClip ringClip; // Clip del teléfono sonando
    [SerializeField] private AudioClip pickupClip; // Clip de atender la llamada
    
    [Header("Phone Objects Reference")]
    [SerializeField] private PhoneOpen phoneOpenScript; // Referencia al script del teléfono abierto
    [SerializeField] private GameObject phoneOpenGameObject; // GameObject del teléfono abierto
    
    private AudioSource audioSource;
    private bool isRinging = false;
    private bool canInteract = false;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ValidateReferences();
    }

    public override void OnHoverEnter()
    {
        // Solo mostrar outline si está sonando y se puede interactuar
        if (!isRinging || !canInteract) return;
        
        base.OnHoverEnter();
    }

    public override void OnHoverExit()
    {
        if (!canInteract) return;
        
        base.OnHoverExit();
    }

    public override void OnInteract()
    {
        if (!isRinging || !canInteract) return;
        
        AnswerCall();
    }

    /// <summary>
    /// Hace que el teléfono empiece a sonar
    /// </summary>
    public void StartRinging()
    {
        if (isRinging) return;
        
        isRinging = true;
        canInteract = true;
        
        if (audioSource != null && ringClip != null)
        {
            audioSource.clip = ringClip;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        Debug.Log("[PhoneClose] Teléfono empezó a sonar");
    }

    /// <summary>
    /// Detiene el sonido del teléfono
    /// </summary>
    public void StopRinging()
    {
        if (!isRinging) return;
        
        isRinging = false;
        
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        
        Debug.Log("[PhoneClose] Teléfono dejó de sonar");
    }

    /// <summary>
    /// Atiende la llamada e inicia la transición
    /// </summary>
    private void AnswerCall()
    {
        Debug.Log("[PhoneClose] Atendiendo llamada...");
        
        StopRinging();
        canInteract = false;
        
        // Reproducir sonido de atender
        if (audioSource != null && pickupClip != null)
        {
            audioSource.clip = pickupClip;
            audioSource.loop = false;
            audioSource.Play();
        }
        
        StartCoroutine(TransitionToPhoneOpen());
    }

    /// <summary>
    /// Corrutina que maneja la transición al teléfono abierto
    /// </summary>
    private IEnumerator TransitionToPhoneOpen()
    {
        // TODO: Implementar fade out aquí
        Debug.Log("[PhoneClose] Iniciando fade out...");
        
        // Esperar un poco para el fade out
        yield return new WaitForSeconds(0.5f);
        
        // Desactivar este GameObject
        gameObject.SetActive(false);
        
        // Activar el teléfono abierto
        if (phoneOpenGameObject != null)
            phoneOpenGameObject.SetActive(true);
        
        // TODO: Implementar movimiento de cámara aquí
        Debug.Log("[PhoneClose] Cambiando posición de cámara...");
        
        // TODO: Implementar fade in aquí
        Debug.Log("[PhoneClose] Iniciando fade in...");
        
        // Esperar un poco para el fade in
        yield return new WaitForSeconds(0.5f);
        
        // Iniciar la llamada en el teléfono abierto
        if (phoneOpenScript != null)
        {
            phoneOpenScript.StartCall();
        }
    }

    /// <summary>
    /// Valida las referencias necesarias
    /// </summary>
    private void ValidateReferences()
    {
        if (ringClip == null)
            Debug.LogWarning("[PhoneClose] Ring clip no asignado");
        
        if (pickupClip == null)
            Debug.LogWarning("[PhoneClose] Pickup clip no asignado");
        
        if (phoneOpenScript == null)
            Debug.LogWarning("[PhoneClose] PhoneOpen script no asignado");
        
        if (phoneOpenGameObject == null)
            Debug.LogWarning("[PhoneClose] PhoneOpen GameObject no asignado");
    }
}
