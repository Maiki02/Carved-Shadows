using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections;

public class PadLockPassword : MonoBehaviour
{
    public int[] _numberPassword = { 0, 0, 0, 0 };
    public TextMeshProUGUI successMessageText;
    public bool passwordResuelto = false;
    public float destroyDelay = 3f;

    private MoveRuller _moveRull;
    private AudioSource _audioSource;
    private Renderer[] _renderers;
    //[SerializeField] private PadlockInspectable _padlockInspectable;

    [SerializeField] private Door doorToOpen;

    private void Awake()
    {
        _moveRull = FindObjectOfType<MoveRuller>();
        _audioSource = GetComponent<AudioSource>();
        _renderers = GetComponentsInChildren<Renderer>();
        //_padlockInspectable = GetComponent<PadlockInspectable>();
    }

    public void Password()
    {

        if (passwordResuelto || !_moveRull._numberArray.SequenceEqual(_numberPassword))
            return;

        Debug.Log("Password correct");
        passwordResuelto = true;

        foreach (var ruller in _moveRull._rullers)
        {
            var emission = ruller.GetComponent<PadLockEmissionColor>();
            if (emission != null)
            {
                emission._isSelect = false;
                emission.BlinkingMaterial();
            }
        }

        if (successMessageText != null)
        {
            successMessageText.text = "¡Candado desbloqueado!";
            successMessageText.gameObject.SetActive(true);
            StartCoroutine(HiddeMessageAfter(2f));
        }

        if (_audioSource != null)
            _audioSource.Play();

        /*if (_padlockInspectable != null)
        {
            //_padlockInspectable.SalirInspeccion();
        }*/ 

        this.doorToOpen.SetType(TypeDoorInteract.NextLevel);

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator HiddeMessageAfter(float segundos)
    {
        yield return new WaitForSeconds(segundos);
        if (successMessageText != null)
            successMessageText.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = destroyDelay;
        float time = 0f;

        foreach (var renderer in _renderers)
        {
            foreach (var mat in renderer.materials)
            {
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            foreach (var renderer in _renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color color = mat.color;
                        color.a = alpha;
                        mat.color = color;
                    }
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}