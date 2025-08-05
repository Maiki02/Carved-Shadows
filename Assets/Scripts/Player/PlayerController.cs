using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;

    public Transform cameraTransform;

    [Header("Camera Walk Effect Settings")]
    [Tooltip("Cámara virtual para caminar")]
    [SerializeField] private CinemachineVirtualCamera walkCamera;
    [Tooltip("Cámara virtual para idle")]
    [SerializeField] private CinemachineVirtualCamera idleCamera;
    [Tooltip("Velocidad máxima que alcanza tu personaje al caminar/correr.")]
    [SerializeField] private float maxSpeed = 5f;
    [Tooltip("La máxima amplitud del 'shake' al caminar a velocidad máxima.")]
    [SerializeField] private float maxAmplitude = 1f;

    private CinemachineBasicMultiChannelPerlin walkNoise;
    private CinemachineBasicMultiChannelPerlin idleNoise;
    private bool wasMoving = false;

    [Header("Freeze Settings")]
    [SerializeField] private Vector3 followPointWhileFrozen;
    [SerializeField] private CinemachineVirtualCamera followCamera; // Cámara que se usa cuando el jugador está congelado

    [Header("Inspection Settings")]
    [SerializeField] private CinemachineVirtualCamera normalCameraPoint; // Punto de referencia para la cámara cuando el jugador no está congelado
    [SerializeField] private CinemachineBrain cinemachineBrain; // Cerebro de Cinemachine para controlar las transiciones de cámara

    [Header("Fall Settings")]
    private float fallDuration = 1f;         // Tiempo total de la caída
    private float finalTiltAngle = 90f;      // Grados que gira sobre el eje X
    private AnimationCurve fallCurve =                          // Curva de aceleración angular
    AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private bool isFalling = false;

    private bool isInspecting = false; // Indica si el jugador está inspeccionando un objeto


    private CharacterController controller;

    private float yaw;
    private float pitch;

    private CinemachinePOV playerPOV;

    private bool controlesActivos = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = GetComponentInChildren<Camera>().transform;

        if (normalCameraPoint != null)
        {
            playerPOV = normalCameraPoint.GetCinemachineComponent<CinemachinePOV>();
        }

        var vCam = GameObject.FindGameObjectWithTag("PlayerVirtualCamera");
        if (vCam != null)
        {
            //vCam.GetComponent<Cinemachine.CinemachineVirtualCamera>().GetComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = GetMouseSensitivity();
        }
        yaw = transform.eulerAngles.y;
        pitch = cameraTransform.localEulerAngles.x;

        // Inicializamos los componentes de ruido de ambas cámaras
        if (walkCamera != null)
            walkNoise = walkCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (idleCamera != null)
            idleNoise = idleCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    void Update()
    {
        if (!controlesActivos || GameFlowManager.Instance.IsInTransition) return;
        this.UpdateCamera();
        this.UpdateMovement();

        UpdateCameraWalkEffect();
    }
    // Efecto de "shake" de cámara al caminar o estar quieto
    private void UpdateCameraWalkEffect()
    {
        if (walkCamera == null || idleCamera == null || controller == null)
            return;

        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        bool isMoving = (speed > 0.1f && controller.isGrounded);

        // Cambia la prioridad de las cámaras según el estado
        if (isMoving != wasMoving)
        {
            // Sincroniza la rotación del POV entre cámaras antes de cambiar prioridades
            if (walkCamera != null && idleCamera != null)
            {
                var walkPOV = walkCamera.GetCinemachineComponent<CinemachinePOV>();
                var idlePOV = idleCamera.GetCinemachineComponent<CinemachinePOV>();
                if (isMoving && walkPOV != null && idlePOV != null)
                {
                    // Copia la rotación de idle a walk
                    walkPOV.m_HorizontalAxis.Value = idlePOV.m_HorizontalAxis.Value;
                    walkPOV.m_VerticalAxis.Value = idlePOV.m_VerticalAxis.Value;
                }
                else if (!isMoving && walkPOV != null && idlePOV != null)
                {
                    // Copia la rotación de walk a idle
                    idlePOV.m_HorizontalAxis.Value = walkPOV.m_HorizontalAxis.Value;
                    idlePOV.m_VerticalAxis.Value = walkPOV.m_VerticalAxis.Value;
                }
            }
            if (isMoving)
            {
                walkCamera.Priority = 20;
                idleCamera.Priority = 10;
            }
            else
            {
                walkCamera.Priority = 10;
                idleCamera.Priority = 20;
            }
            wasMoving = isMoving;
        }

        // Ajusta el grain solo en la cámara activa
        if (isMoving && walkNoise != null)
        {
            float targetAmplitude = maxAmplitude * (speed / maxSpeed);
            walkNoise.m_AmplitudeGain = Mathf.Lerp(walkNoise.m_AmplitudeGain, targetAmplitude, Time.deltaTime * 5f);
        }
        else if (!isMoving && idleNoise != null)
        {
            float targetAmplitude = 1f;
            idleNoise.m_AmplitudeGain = Mathf.Lerp(idleNoise.m_AmplitudeGain, targetAmplitude, Time.deltaTime * 5f);
        }
    }

    float GetMouseSensitivity()
    {
return 700f;
        //return GameController.Instance.MouseSensitivity;
    }

    public float GetFallDuration()
    {
        return fallDuration;
    }

    private void UpdateCamera()
    {
        //Capturamos los movimientos del ratón
        float mouseX = Input.GetAxis("Mouse X") * GetMouseSensitivity() * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * GetMouseSensitivity() * Time.deltaTime;

        //Calculamos la rotación de la cámara
        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        //Aplicamos la rotación de la cámara
        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void UpdateMovement()
    {
        //if (!controller.enabled || GameFlowManager.Instance.IsInTransition) return; // Si el controller está desactivado, no movemos al jugador

        //Capturamos los movimientos del teclado
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        //Aplicamos más velocidad si el shift está pulsado
        /*if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveX *= 8f;
            moveZ *= 8f;
        }*/

        //Calculamos la dirección de movimiento
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        //Aplicamos la gravedad
        move.y -= 9.81f * Time.deltaTime;

        //Movemos al jugador
        controller.Move(move);
    }

    public void FallToTheGround()
    {
        if (!isFalling)
            StartCoroutine(FallCoroutine());
    }


    private IEnumerator FallCoroutine()
    {
        isFalling = true;
        this.controlesActivos = false;  // desactiva los controles del jugador
        controller.enabled = false;  // desactiva el controller durante la animación

        Vector3 startPos = transform.position;
        // Calcula el pivot en el borde inferior (para que rote “sobre el suelo”)
        float pivotOffset = (controller.height * 0.5f) - controller.radius;
        Vector3 pivot = startPos + Vector3.down * pivotOffset;

        float elapsed = 0f;
        float prevCurveVal = 0f;

        while (elapsed < fallDuration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            float tNorm = Mathf.Clamp01(elapsed / fallDuration);
            float curveV = fallCurve.Evaluate(tNorm);

            // Ángulo diferencial a aplicar este frame
            float deltaAngle = (curveV - prevCurveVal) * finalTiltAngle;

            // Rota alrededor del pivot usando el eje forward (caída lateral)
            transform.RotateAround(pivot, transform.forward, deltaAngle);

            prevCurveVal = curveV;
            yield return null;
        }

        float finalDelta = finalTiltAngle - (prevCurveVal * finalTiltAngle);
        if (!Mathf.Approximately(finalDelta, 0f))
            transform.RotateAround(pivot, transform.forward, finalDelta);

        isFalling = false;
        this.controlesActivos = true;  // reactiva los controles del jugador
        controller.enabled = true;
    }

    public void SetControlesActivos(bool activos)
    {
        controlesActivos = activos;

        Cursor.lockState = activos
            ? CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = !activos;

        if (playerPOV != null)
            playerPOV.enabled = activos;
    }



    public void SetStatusCharacterController(bool status)
    {
        if (controller != null)
        {
            controller.enabled = status;
        }
    }

    /* Dado un determinado tiempo, inmovilizamos al jugador y lo hacemos seguir un punto */
    void FreezePlayerAndFaceEnemies()
    {
        //Freezamos al jugador
        this.controlesActivos = false;

        // this.normalCameraPoint.gameObject.SetActive(false);
        this.followCamera.gameObject.SetActive(true);



    }

    public void ActivarCamaraInspeccion(Transform inspectionPoint)
    {
        this.SetControlesActivos(false);
        GameController.Instance.IsInspecting = true;
        //Debug.Log("Activando cámara de inspección");
        this.cinemachineBrain.enabled = false;
        this.normalCameraPoint.gameObject.SetActive(false);
    }

    public void DesactivarCamaraInspeccion()
    {
        this.SetControlesActivos(true);
        GameController.Instance.IsInspecting = false;
        this.cinemachineBrain.enabled = true;
        //this.inspectionCamera.gameObject.SetActive(false);
        this.normalCameraPoint.gameObject.SetActive(true);
    }

}
