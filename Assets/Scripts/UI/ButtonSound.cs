using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Instancia única global AudioSource
    private static AudioSource globalAudioSource;

    void Awake()
    {

        if (globalAudioSource == null)
        {

            GameObject soundManager = new GameObject("GlobalButtonSoundManager");
            globalAudioSource = soundManager.AddComponent<AudioSource>();
            globalAudioSource.playOnAwake = false;
            DontDestroyOnLoad(soundManager);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && globalAudioSource != null)
        {
            EnsureAudioSourceIsEnabled();
            globalAudioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && globalAudioSource != null)
        {
            EnsureAudioSourceIsEnabled();
            globalAudioSource.PlayOneShot(clickSound);
        }
    }

    private void EnsureAudioSourceIsEnabled()
    {
        if (!globalAudioSource.enabled)
        {
            globalAudioSource.enabled = true;
        }
    }
}
