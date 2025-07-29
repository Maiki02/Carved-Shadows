using System.Collections;
using UnityEngine;

public class Door_NextLoop : ObjectInteract
{
    private bool isTransitioning = false;

    public override void OnInteract()
    {
        if (!isTransitioning)
        {
            HandleTransition();
        }
    }

    private void HandleTransition()
    {
        isTransitioning = true;
        GameFlowManager.Instance.SetTransitionStatus(true);

        var player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetControlesActivos(false);
            player.SetStatusCharacterController(false);
        }

        // Activar la escena precargada en el GameFlow
        GameController.Instance.NextLevel();
        GameFlowManager.Instance.ActivatePreloadedScene();
        // Acordarse de poner video en la escena nueva
    }
}
