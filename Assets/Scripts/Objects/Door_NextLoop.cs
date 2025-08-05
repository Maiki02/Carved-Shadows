using System.Collections;
using UnityEngine;

public class Door_NextLoop : ObjectInteract
{
    private bool isTransitioning = false;
    [SerializeField] private Transform doorEntryPoint; 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotateSpeed = 5f;

    public override void OnInteract()
    {
        if (!isTransitioning)
        {
            StartCoroutine(ApproachAndTransition());
        }
    }

    private IEnumerator ApproachAndTransition()
    {
        isTransitioning = true;
        GameFlowManager.Instance.SetTransitionStatus(true);

        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) yield break;

        var player = playerObj.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetControlesActivos(false);
            player.SetStatusCharacterController(false);
        }

        // Mover al jugador hacia el punto de entrada de la puerta
        if (doorEntryPoint != null && playerObj != null)
        {
            while (Vector3.Distance(playerObj.transform.position, doorEntryPoint.position) > 0.05f)
            {
                Vector3 dir = (doorEntryPoint.position - playerObj.transform.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                playerObj.transform.rotation = Quaternion.Slerp(playerObj.transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

                playerObj.transform.position = Vector3.MoveTowards(playerObj.transform.position, doorEntryPoint.position, moveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        // Esperar un breve momento antes de cambiar de escena
        yield return new WaitForSeconds(0.3f);

        GameController.Instance.NextLevel();
        GameFlowManager.Instance.ActivatePreloadedScene();
    }
}
