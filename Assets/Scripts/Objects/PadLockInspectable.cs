
using UnityEngine;

public class PadlockInspectable : InspectableObject
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Rigidbody rb;
    private Collider col;
    private PadLockPassword padlockPassword;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        padlockPassword = GetComponentInChildren<PadLockPassword>();

    }

    void Update()
    {
        if (isInspecting)
        {
            this.tiempoDesdeInicio += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.E) && tiempoDesdeInicio > 0.2f)
            {
                SalirInspeccion();
                this.tiempoDesdeInicio = 0f;
            }
        }
    }

    public override void OnInteract()
    {
        EntrarInspeccion();

        if (EstaSiendoInspeccionado())
        {
            Debug.Log("Modo inspección del candado iniciado.");
        }
        else
        {
            Debug.LogWarning("No se inició la inspección del candado");
        }

    }


    public void EntrarInspeccion()
    {
        if (padlockPassword != null && padlockPassword.passwordResuelto) return;
        if (isInspecting) return;

        isInspecting = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        // 1) Desanidar
        transform.SetParent(null);

        // 2) Teleportar al punto
        transform.position = inspectionPoint.position;

        // 3) Alinear sólo yaw con la cámara
        Vector3 flatF = Camera.main.transform.forward;
        flatF.y = 0f;
        if (flatF.sqrMagnitude < 0.001f) flatF = Vector3.forward;
        transform.rotation = Quaternion.LookRotation(-flatF, Vector3.up);

        // 4) Desactivar físicas/colisiones
        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;

        // 5) Activar tus rollers
        foreach (var r in GetComponentsInChildren<MoveRuller>(true))
            r.isActive = true;

        // 6) Subir cámara de inspección, controles, etc.
        NivelarCamara();
        ActivarInspeccion();
    }



    public void SalirInspeccion()
    {
        if(!isInspecting) return;
        isInspecting = false;

        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        if (col != null)
            col.enabled = true;

        // Desactivar rulos
        MoveRuller[] rullers = GetComponentsInChildren<MoveRuller>(true);
        foreach (var r in rullers)
            r.isActive = false;

        // Reactivar control del jugador
        this.DesactivarInspeccion();
    }

    /*public bool EstaSiendoInspeccionado()
    {
        return isInspecting;
    }*/
}
