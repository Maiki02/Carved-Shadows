using UnityEngine;

public class Loop4_Trigger1 : MonoBehaviour
{
    [SerializeField] private Statue statue;
    [SerializeField] private AudioSource womanCryingAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            statue.TriggerNextStep();
            womanCryingAudio.Stop();
            gameObject.SetActive(false); // para que no se reactive
        }
    }
}
