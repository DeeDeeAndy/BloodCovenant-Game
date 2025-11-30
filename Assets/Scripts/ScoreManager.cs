using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    private int currentScore = 0;
    private int highScore = 0;

    [Header("Multiplier Settings")]
    public float multiplierDecayTime = 3f;
    public int maxMultiplier = 20;
    private int currentMultiplier = 1;
    private Coroutine decayCoroutine;

    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnMultiplierChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void AddScore(int basePoints)
    {
        int points = basePoints * currentMultiplier;
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);

        IncreaseMultiplier();

        Debug.Log($"+{points} points! (x{currentMultiplier}) Total: {currentScore}");
    }

    void IncreaseMultiplier()
    {
        if (currentMultiplier < maxMultiplier)
        {
            currentMultiplier++;
            OnMultiplierChanged?.Invoke(currentMultiplier);
        }

        if (decayCoroutine != null)
            StopCoroutine(decayCoroutine);

        decayCoroutine = StartCoroutine(MultiplierDecay());
    }

    IEnumerator MultiplierDecay()
    {
        yield return new WaitForSeconds(multiplierDecayTime);

        while (currentMultiplier > 1)
        {
            currentMultiplier--;
            OnMultiplierChanged?.Invoke(currentMultiplier);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ResetMultiplier()
    {
        if (decayCoroutine != null)
            StopCoroutine(decayCoroutine);

        currentMultiplier = 1;
        OnMultiplierChanged?.Invoke(currentMultiplier);
    }

    public void SaveHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public int GetScore() => currentScore;
    public int GetMultiplier() => currentMultiplier;
    public int GetHighScore() => highScore;
}