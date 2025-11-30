using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoreManager : MonoBehaviour
{
    public static LoreManager Instance { get; private set; }

    [Header("Settings")]
    public float textDisplayDuration = 8f;

    private AudioSource audioSource;
    private List<string> collectedLore = new List<string>();

    public event System.Action<string> OnLoreDisplayed;
    public event System.Action OnLoreHidden;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ShowLore(string text, AudioClip audioClip = null)
    {
        if (!collectedLore.Contains(text))
            collectedLore.Add(text);

        OnLoreDisplayed?.Invoke(text);
        Debug.Log($"LORE: {text}");

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
            StartCoroutine(HideLoreAfterAudio(audioClip.length));
        }
        else
        {
            StartCoroutine(HideLoreAfterDelay(textDisplayDuration));
        }
    }

    IEnumerator HideLoreAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnLoreHidden?.Invoke();
    }

    IEnumerator HideLoreAfterAudio(float audioLength)
    {
        yield return new WaitForSeconds(audioLength + 1f);
        OnLoreHidden?.Invoke();
    }

    public List<string> GetCollectedLore() => collectedLore;
    public int GetLoreCount() => collectedLore.Count;
}