using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AmbientSound
{
    public string name;
    public AudioSource source;
    public bool isEnabled;
    public float startDelay;
    public bool loop;
    public bool randomizeStart;

    public bool enableFadeOut = false;
    public float fadeOutStartTime = 0f;
    public float fadeOutDuration = 2f;


}

public class AmbientSoundController : MonoBehaviour
{
    public List<AmbientSound> ambientSounds;

    private void Start()
    {
        foreach (var sound in ambientSounds)
        {
            if (sound.isEnabled)
            {
                StartCoroutine(PlayWithDelay(sound));
            }
        }
    }

    private IEnumerator PlayWithDelay(AmbientSound sound)
    {
        float delay = sound.startDelay;
        if (sound.randomizeStart)
        {
            delay += Random.Range(0f, 5f); // podés ajustar el rango
        }

        yield return new WaitForSeconds(delay);

        sound.source.loop = sound.loop;
        sound.source.Play();

        if (sound.enableFadeOut)
        {
            StartCoroutine(HandleFadeOut(sound));
        }
    }

    public void FadeOutAll(float fadeTime = 2f)
    {
        foreach (var sound in ambientSounds)
        {
            if (sound.source.isPlaying)
            {
                StartCoroutine(FadeOut(sound.source, fadeTime));
            }
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator HandleFadeOut(AmbientSound sound)
    {
        yield return new WaitForSeconds(sound.fadeOutStartTime);
        yield return FadeOut(sound.source, sound.fadeOutDuration);
    }

}
