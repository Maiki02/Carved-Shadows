using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [Tooltip("Transform del punto al que queremos teleportar")]
    [SerializeField] private Transform pointToTeleport;
    [SerializeField] private Door door; // Puerta con la que tiene que interactuar el jugador
    [SerializeField] private Door initialDoor; // Puerta inicial del juego

    [Header("Configuración de las habitaciones de cada nivel")]
    [SerializeField] private List<GameObject> rooms; // Referencia a las habitaciones

    [Header("Puerta museo 1 y 2")]
    [SerializeField] private Door puertaMuseo1;
    [SerializeField] private Door puertaMuseo2;

    public bool IsInTransition { get; private set; }

    private GameObject player; // Referencia al jugador

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No se encontró el objeto Player en la escena.");
        }
    }

    void Start()
    {
        AudioController.Instance.PlayMusic(AudioType.MusicLevel1, true);
    }

    public void GoToNextLevel()
    {
        StartCoroutine(NextLevel(this.player));
    }

    private IEnumerator NextLevel(GameObject player)
    {
        IsInTransition = true;

        // Ejecuta toda la secuencia (oscurecer, caer, teleport, abrir ojos)

        yield return StartCoroutine(SubirNivel());


        yield return StartCoroutine(
            FadeManager.Instance.FaintAndLoadRoutine(player, pointToTeleport)
        );

        SetTypeOfDoor();
        IsInTransition = false;
    }

    private IEnumerator SubirNivel()
    {
        yield return new WaitForSeconds(0.5f); // Espera un segundo antes de subir de nivel
           
                // Ahora que ya terminó todo eso, sube de nivel, activa puertas y rooms
        GameController.Instance.NextLevel();
        
        ActivateRoom(GameController.Instance.CurrentLevel);
    }



    private void ActivateRoom(int level)
    {
        if (level <= 0 || level > rooms.Count)
        {
            Debug.LogError("Nivel fuera de rango: " + level);
            return;
        }

        // Desactivar todas las habitaciones
        foreach (GameObject room in rooms)
        {
            room.SetActive(false);
        }

        // Activar la habitación correspondiente al nivel actual
        rooms[level - 1].SetActive(true);
    }

    private void ActivateMusic(int level)
    {
        switch (level)
        {
            case 1:
                AudioController.Instance.PlayMusic(AudioType.MusicLevel1, true);
                break;
            case 2:
                AudioController.Instance.PlayMusic(AudioType.MusicLevel2, true);
                break;
            case 3:
                AudioController.Instance.PlayMusic(AudioType.MusicLevel3, true);
                break;
            default:
                Debug.LogWarning("Nivel no reconocido para la música: " + level);
                break;
        }
    }

    private void SetTypeOfDoor()
    {
        if (door == null || initialDoor == null)
        {
            Debug.LogError("No se ha asignado una puerta o puerta inicial al GameFlowManager.");
            return;
        }

        switch (GameController.Instance.CurrentLevel)
        {
            case 1:
                door.SetType(TypeDoorInteract.Key);
                break;
            case 2:
                door.SetType(TypeDoorInteract.None);
                break;
            case 3:
                door.SetType(TypeDoorInteract.None);
                break;
            case 4:
                door.SetType(TypeDoorInteract.None);
                break;
            default:
                Debug.LogWarning("Nivel no reconocido para la puerta: " + GameController.Instance.CurrentLevel);
                break;
        }
        if(GameController.Instance.CurrentLevel == 2)
        {
            this.puertaMuseo1.OpenOrCloseDoor(false); // Aseguramos que la puerta del museo 1 esté cerrada al inicio
            this.puertaMuseo2.OpenOrCloseDoor(false); // Aseguramos que la puerta del museo 2 esté cerrada al inicio
            this.initialDoor.OpenOrCloseDoor(false); // Aseguramos que la puerta inicial esté cerrada al inicio

            puertaMuseo1.SetType(TypeDoorInteract.None);
            puertaMuseo2.SetType(TypeDoorInteract.None);
            initialDoor.SetType(TypeDoorInteract.None); // La puerta inicial siempre es de tipo None

        }
    }

}
