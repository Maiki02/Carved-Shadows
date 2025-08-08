using UnityEngine;

public class Loop4_Trigger2 : MonoBehaviour
{
    [SerializeField] private Statue statue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            statue.TriggerNextStep();
            gameObject.SetActive(false);
        }
    }
}