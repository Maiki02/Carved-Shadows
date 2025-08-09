using System.Collections;
using UnityEngine;

public class Statue : MonoBehaviour
{
    [System.Serializable]
    public class StatuePoint
    {
        public Transform targetPoint;
        public float moveSpeed = 2f;
        public bool rotateToTarget = false;
    }

    [SerializeField] private StatuePoint[] points;
    [SerializeField] private bool deactivateOnFinish = false;
    [SerializeField] private Door doorToClose;


    private int currentStep = 0;
    private bool isMoving = false;

    public void TriggerNextStep()
    {
        if (isMoving) return;

        if (currentStep < points.Length)
        {
            StartCoroutine(MoveToPoint(points[currentStep]));
            currentStep++;
        }
        else
        {
            Debug.Log("[Statue] No hay más pasos para ejecutar.");

            if (doorToClose != null)
            {
                StartCoroutine(WaitAndDeactivateAfterDoorCloses());
            }
            else if (deactivateOnFinish)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator WaitAndDeactivateAfterDoorCloses()
    {
        doorToClose.StartSlowClosing();

        float waitTime = doorToClose.SlowCloseDuration;
        yield return new WaitForSeconds(waitTime);

        if (deactivateOnFinish)
        {
            Debug.Log("[Statue] Desactivando estatua después de cerrar la puerta.");
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveToPoint(StatuePoint point)
    {
        isMoving = true;

        if (point.targetPoint == null)
        {
            isMoving = false;
            yield break;
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = point.targetPoint.position;
        targetPos.y = startPos.y;

        while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPos.x, 0, targetPos.z)) > 0.05f)
        {
            Vector3 nextPos = Vector3.MoveTowards(
                new Vector3(transform.position.x, 0, transform.position.z),
                new Vector3(targetPos.x, 0, targetPos.z),
                point.moveSpeed * Time.deltaTime
            );

            transform.position = new Vector3(nextPos.x, startPos.y, nextPos.z);

            if (point.rotateToTarget)
            {
                Vector3 dir = (targetPos - transform.position);
                dir.y = 0f;
                if (dir != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, point.moveSpeed * Time.deltaTime);
                }
            }

            yield return null;
        }

        transform.position = targetPos;

        if (point.rotateToTarget)
        {
            Vector3 dir = (targetPos - transform.position);
            dir.y = 0f;
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir);
            }
        }
        isMoving = false;

        if (currentStep >= points.Length && doorToClose != null)
        {
            Debug.Log("[Statue] Recorrido completo. Cerrando la puerta.");
            doorToClose.StartSlowClosing();
        }

    }
}
