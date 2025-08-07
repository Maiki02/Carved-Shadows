using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootstepAudioManager : MonoBehaviour
{
    [Header("Clips de pasos")]
    public List<AudioClip> pasos;
    public List<AudioClip> beats;

    [Header("Clips de pasos para alfombra")]
    public List<AudioClip> alfombraPasos;
    public List<AudioClip> alfombraBeats;

    [Header("Audio Sources")]
    public AudioSource pasosSource;
    public AudioSource beatsSource;

    [Header("Audio Mixer")]
    public AudioMixerGroup pasosMixer;
    public AudioMixerGroup beatsMixer;

    [Header("Configuración de pasos")]
    public float stepInterval = 0.5f;
    public float minMovementTime = 0.15f;

    private float stepTimer;
    private float movementTime;
    private int lastIndex = -1;
    private bool sobreAlfombra = false;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponentInParent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("No se encontró CharacterController en el objeto padre.");
        }

        if (pasosSource != null && pasosMixer != null)
            pasosSource.outputAudioMixerGroup = pasosMixer;

        if (beatsSource != null && beatsMixer != null)
            beatsSource.outputAudioMixerGroup = beatsMixer;
    }

    void Update()
    {
        if (characterController == null) return;

        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        bool isWalking = horizontalVelocity.magnitude > 0.1f && characterController.isGrounded;

        if (isWalking)
        {
            movementTime += Time.deltaTime;
            stepTimer -= Time.deltaTime;

            if (movementTime >= minMovementTime && stepTimer <= 0f)
            {
                PlayRandomStepPair();
                stepTimer = stepInterval;
            }
        }
        else
        {
            movementTime = 0f;
            stepTimer = 0f;
        }
    }

    void PlayRandomStepPair()
    {
        List<AudioClip> currentPasos = sobreAlfombra ? alfombraPasos : pasos;
        List<AudioClip> currentBeats = sobreAlfombra ? alfombraBeats : beats;

        if (currentPasos.Count == 0 || currentBeats.Count == 0) return;

        int index;
        do
        {
            index = Random.Range(0, currentPasos.Count);
        } while (index == lastIndex && currentPasos.Count > 1);
        lastIndex = index;

        float randomPitch = Random.Range(0.95f, 1.05f);
        pasosSource.pitch = randomPitch;
        beatsSource.pitch = randomPitch;

        pasosSource.clip = currentPasos[index];
        beatsSource.clip = currentBeats[index];

        pasosSource.Play();
        beatsSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Alfombra"))
        {
            sobreAlfombra = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Alfombra"))
        {
            sobreAlfombra = false;
        }
    }
}
