using System.Collections;
using UnityEngine;
using Cinemachine;

public class Phone : ObjectInteract
{
    private PlayerController playerController;
    [Header("Phone Call Settings")]

    [Tooltip("Secuencia de diálogos de la llamada telefónica")]
    [SerializeField] private DialogMessage[] callDialogSequence;

    [Tooltip("Priority to set for the phone camera during the call.")]
    public int phoneCameraPriority = 20;

    private CinemachineVirtualCamera phoneCamera;
    private bool isCalling = false;

    protected override void Awake()
    {
        base.Awake();
        // Busca la cámara virtual con el tag "CameraPhone" en los hijos
        foreach (var cam in GetComponentsInChildren<CinemachineVirtualCamera>(true))
        {
            if (cam.CompareTag("CameraPhone"))
            {
                phoneCamera = cam;
                break;
            }
        }
        if (phoneCamera == null)
        {
            Debug.LogError($"No CinemachineVirtualCamera with tag 'CameraPhone' found in {gameObject.name}.");
        }

        // Busca el PlayerController por tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("PlayerController component not found on Player object.");
            }
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found.");
        }
    }

    public override void OnInteract()
    {
        if (isCalling || phoneCamera == null) return;
        StartCoroutine(PhoneCallRoutine());
    }

    private IEnumerator PhoneCallRoutine()
    {
        Debug.Log("Is calling...");
        if(isCalling) yield break;

        isCalling = true;
        // Desactiva controles del Player
        if (playerController != null)
            playerController.SetControlesActivos(false);

        // Sube la prioridad para activar la cámara del teléfono
        phoneCamera.Priority = phoneCameraPriority;

        // Muestra la secuencia de diálogos y espera a que termine
        if (callDialogSequence != null && callDialogSequence.Length > 0)
        {
            yield return StartCoroutine(ShowDialogSequenceAndWait());
        }

        // Baja la prioridad de la cámara del teléfono para devolver el control al Player
        Debug.Log("Llamada terminada");
        phoneCamera.Priority = 0;
        // Reactiva controles del Player
        if (playerController != null)
            playerController.SetControlesActivos(true);
        isCalling = false;
    }

    // Corrutina auxiliar para esperar a que termine la secuencia de diálogos
    private IEnumerator ShowDialogSequenceAndWait()
    {
        bool finished = false;
        // Llama a la secuencia y espera su finalización
        yield return StartCoroutine(DialogSequenceCoroutine(() => finished = true));
        while (!finished) yield return null;
    }

    // Corrutina que ejecuta la secuencia y llama al callback al terminar
    private IEnumerator DialogSequenceCoroutine(System.Action onComplete)
    {
        DialogController.Instance.ShowDialogSequence(callDialogSequence);
        float totalDuration = 0f;
        foreach (var msg in callDialogSequence)
            totalDuration += msg.duration;
        yield return new WaitForSeconds(totalDuration);
        onComplete?.Invoke();
    }
}
