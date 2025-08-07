using System.Collections;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float GetFallDuration() => fallTime;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 1.75f;
    private Vector3 velocity;
    private Vector3 currentMove = Vector3.zero;
    [SerializeField] private float accelerationSpeed = 5f;


    [Header("Cámaras")]
    [SerializeField] private CinemachineVirtualCamera mainCam;
    [SerializeField] private CinemachineVirtualCamera inspectionCam;
    [SerializeField] private CinemachineVirtualCamera freezeCam;
    [SerializeField] private CinemachineBrain brain;

    [Header("Shake al caminar")]
    [SerializeField] private float idleAmplitude = 0.05f;
    [SerializeField] private float maxShake = 2.5f;
    [SerializeField] private float maxSpeed = 3.5f;
    private CinemachinePOV mainPOV;
    private CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] private NoiseSettings walkNoiseProfile;

    [Header("Caída")]
    [SerializeField] private float fallTime = 1f;
    [SerializeField] private float fallAngle = 90f;
    private AnimationCurve fallCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private CharacterController controller;
    private Transform camTransform;
    private bool controlesActivos = true;
    private bool isFalling = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        camTransform = GetComponentInChildren<Camera>().transform;

        if (mainCam)
        {
            mainPOV = mainCam.GetCinemachineComponent<CinemachinePOV>();
            noise = mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_NoiseProfile = walkNoiseProfile;
        }
    }


    void Update()
    {

        AplicarShakeCamara();

        if (!controlesActivos || isFalling || GameFlowManager.Instance.IsInTransition) return;

        UpdateSensibilidad();
        MoverJugador();
    }

    private void UpdateSensibilidad()
    {
        float sens = GameController.Instance.MouseSensitivity;
        if (mainPOV != null)
        {
            mainPOV.m_HorizontalAxis.m_MaxSpeed = sens;
            mainPOV.m_VerticalAxis.m_MaxSpeed = sens;
        }
    }


    private void MoverJugador()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * inputX + forward * inputZ;
        Vector3 targetMove = direction * moveSpeed;
        currentMove = Vector3.Lerp(currentMove, targetMove, Time.deltaTime * accelerationSpeed);


        if (controller.isGrounded)
            velocity.y = -2f;
        else
            velocity.y += Physics.gravity.y * Time.deltaTime;

        Vector3 finalMove = currentMove;
        finalMove.y = velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        controller.Move(finalMove * Time.deltaTime);
    }


    private void AplicarShakeCamara()
    {
        float speed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        bool isMoving = speed > 0.1f && controller.isGrounded;

        float targetAmp = isMoving
            ? Mathf.Clamp01(speed / maxSpeed) * maxShake
            : idleAmplitude;

        noise.m_AmplitudeGain = Mathf.Lerp(
            noise.m_AmplitudeGain,
            targetAmp,
            Time.deltaTime * 5f
        );
    }



    public void SetControlesActivos(bool activos)
    {
        controlesActivos = activos;

        Cursor.lockState = activos ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !activos;
    }

    public void SetCamaraActiva(bool activa)
    {
        if (mainPOV == null) return;

        if (activa)
        {
            float sens = GameController.Instance.MouseSensitivity;
            mainPOV.m_HorizontalAxis.m_MaxSpeed = sens;
            mainPOV.m_VerticalAxis.m_MaxSpeed = sens;
        }
        else
        {
            mainPOV.m_HorizontalAxis.m_MaxSpeed = 0f;
            mainPOV.m_VerticalAxis.m_MaxSpeed = 0f;
        }
    }


    public void FallToTheGround()
    {
        if (!isFalling)
            StartCoroutine(FallCoroutine());
    }

    private IEnumerator FallCoroutine()
    {
        isFalling = true;
        SetControlesActivos(false);
        controller.enabled = false;

        Vector3 pivot = transform.position + Vector3.down * ((controller.height * 0.5f) - controller.radius);
        float elapsed = 0f, prevCurve = 0f;

        while (elapsed < fallTime)
        {
            float t = Mathf.Clamp01(elapsed / fallTime);
            float curve = fallCurve.Evaluate(t);
            float delta = (curve - prevCurve) * fallAngle;

            transform.RotateAround(pivot, transform.forward, delta);
            prevCurve = curve;

            elapsed += Time.deltaTime;
            yield return null;
        }

        float final = fallAngle - (prevCurve * fallAngle);
        if (!Mathf.Approximately(final, 0f))
            transform.RotateAround(pivot, transform.forward, final);

        controller.enabled = true;
        SetControlesActivos(true);
        isFalling = false;
    }

    public void ActivarCamaraInspeccion(Transform inspectionPoint)
    {
        this.SetControlesActivos(false);
        GameController.Instance.IsInspecting = true;
        if (brain) brain.enabled = false;
        if (inspectionCam) inspectionCam.gameObject.SetActive(true);
    }

    public void DesactivarCamaraInspeccion()
    {
        GameController.Instance.IsInspecting = false;
        SetControlesActivos(true);
        if (brain) brain.enabled = true;
        if (inspectionCam) inspectionCam.gameObject.SetActive(false);
    }

    public void SetStatusCharacterController(bool status)
    {
        if (controller) controller.enabled = status;
    }

    public Transform GetCameraTransform()
    {
        return camTransform;
    }
}
