using System.Collections;
using UnityEngine;
using Cinemachine;

public class Phone : ObjectInteract
{
    private PlayerController playerController;
    [Header("Phone Call Settings")]
    [Tooltip("Duration of the phone call in seconds.")]
    public float callDuration = 5f;

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
        // Espera la duración de la llamada
        Debug.Log("Duración de la llamada: " + callDuration);
        yield return new WaitForSeconds(callDuration);
        // Baja la prioridad de la cámara del teléfono para devolver el control al Player
        Debug.Log("Llamada terminada");
        phoneCamera.Priority = 0;
        // Reactiva controles del Player
        if (playerController != null)
            playerController.SetControlesActivos(true);
        isCalling = false;
    }
}
