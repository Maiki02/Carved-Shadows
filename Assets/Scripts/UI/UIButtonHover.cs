using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverSound : MonoBehaviour, IPointerEnterHandler
{
    public AudioClip hoverClip;
    private AudioSource audioSource;

    void Start()
    {
        // Puedes usar un AudioSource global o uno por botón
        audioSource = FindObjectOfType<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }
    }
}