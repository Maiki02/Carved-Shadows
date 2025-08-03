using System.Collections;
using UnityEngine;

// El eje Y de la estatua no se moverá durante la animación, solo X y Z cambiarán.
public class Statue : MonoBehaviour
{
    [System.Serializable]
    public class StatuePoint
    {
        public Transform targetPoint;
        public float moveSpeed = 2f;
        public bool rotateToTarget = false;
    }

    [SerializeField]
    private StatuePoint[] points;
    private bool isMoving = false;

    [Header("Comportamiento al finalizar")]
    [Tooltip("Si está activo, la estatua desaparecerá (SetActive(false)) al terminar el recorrido. Si no, permanecerá visible.")]
    [SerializeField]
    private bool deactivateOnFinish = false;


    public void Start()
    {
        this.StartStatueMovement(); 
    }

    // Llamar a esta función desde el trigger externo
    public void StartStatueMovement()
    {
        if (!isMoving && points != null && points.Length > 0)
        {
            StartCoroutine(MoveAlongPoints());
        }
    }

    private IEnumerator MoveAlongPoints()
    {
        isMoving = true;
        for (int i = 0; i < points.Length; i++)
        {
            StatuePoint point = points[i];
            yield return StartCoroutine(MoveToPoint(point));
        }
        isMoving = false;
        if (deactivateOnFinish)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveToPoint(StatuePoint point)
    {
        if (point.targetPoint == null)
            yield break;

        Vector3 startPos = transform.position;
        Vector3 targetPos = point.targetPoint.position;
        // Mantener el Y original de la estatua
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
        // Snap to final position/rotación
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
    }
}
