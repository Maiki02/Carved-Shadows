using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToMenuController : MonoBehaviour
{

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
        Cursor.visible = true; // Hace visible el cursor   
    }


    public void BackToMenu()
    {
        Debug.Log("Regresando al menú principal...");
        GameController.Instance.ResetValues();
        SceneController.Instance.LoadMenuScene();
    }
}
