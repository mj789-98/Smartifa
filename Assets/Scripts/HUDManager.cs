using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI coinScoreText;
    [SerializeField] private GameObject[] lifeIcons;
    [SerializeField] private Image comboTimerFill;
    [SerializeField] private GameObject comboPanel;

    [Header("Animation Settings")]
    [SerializeField] private float coinScoreDisplayDuration = 1f;
    [SerializeField] private float coinScoreFloatSpeed = 100f;
    [SerializeField] private float coinScoreFadeSpeed = 1f;
    [SerializeField] private AnimationCurve coinScoreAlphaCurve;

    [Header("Style Settings")]
    [SerializeField] private string scorePrefix = "Score: ";
    [SerializeField] private string comboPrefix = "x";
    [SerializeField] private Color normalComboColor = Color.white;
    [SerializeField] private Color maxComboColor = Color.yellow;
    [SerializeField] private float maxComboColorPulseSpeed = 2f;

    private ScoringSystem scoringSystem;
    private LivesSystem livesSystem;
    private float maxComboMultiplier;

    private void Awake()
    {
        // Get references
        scoringSystem = FindObjectOfType<ScoringSystem>();
        livesSystem = FindObjectOfType<LivesSystem>();

        if (scoringSystem != null)
        {
            // Subscribe to scoring events
            scoringSystem.onScoreChanged.AddListener(UpdateScore);
            scoringSystem.onComboMultiplierChanged.AddListener(UpdateCombo);
            scoringSystem.onCoinCollected.AddListener(ShowCoinScore);
            maxComboMultiplier = scoringSystem.GetComboMultiplier();
        }
        else
        {
            Debug.LogError("ScoringSystem not found!");
        }

        if (livesSystem != null)
        {
            // Subscribe to lives events
            livesSystem.onLivesChanged.AddListener(UpdateLives);
        }
        else
        {
            Debug.LogError("LivesSystem not found!");
        }

        // Initialize UI
        if (comboPanel != null)
            comboPanel.SetActive(false);
    }

    private void Update()
    {
        UpdateComboTimer();
        UpdateComboPulse();
    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{scorePrefix}{score:N0}";
        }
    }

    private void UpdateCombo(float multiplier)
    {
        if (comboText != null)
        {
            comboText.text = $"{comboPrefix}{multiplier:F1}";
        }

        // Show/hide combo panel
        if (comboPanel != null)
        {
            comboPanel.SetActive(multiplier > 1f);
        }

        maxComboMultiplier = Mathf.Max(maxComboMultiplier, multiplier);
    }

    private void UpdateComboTimer()
    {
        if (comboTimerFill != null && scoringSystem != null)
        {
            float remainingTime = scoringSystem.GetComboTimeRemaining();
            float normalizedTime = Mathf.Clamp01(remainingTime / 2f); // Assuming 2 second window
            comboTimerFill.fillAmount = normalizedTime;
        }
    }

    private void UpdateComboPulse()
    {
        if (comboText != null && scoringSystem != null)
        {
            float currentMultiplier = scoringSystem.GetComboMultiplier();
            if (currentMultiplier >= maxComboMultiplier)
            {
                // Pulse color at max combo
                float pulse = (Mathf.Sin(Time.time * maxComboColorPulseSpeed) + 1f) * 0.5f;
                comboText.color = Color.Lerp(normalComboColor, maxComboColor, pulse);
            }
            else
            {
                // Lerp color based on combo progress
                float t = (currentMultiplier - 1f) / (maxComboMultiplier - 1f);
                comboText.color = Color.Lerp(normalComboColor, maxComboColor, t);
            }
        }
    }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
            {
                lifeIcons[i].SetActive(i < lives);
            }
        }
    }

    private void ShowCoinScore(int score)
    {
        if (coinScoreText != null)
        {
            StartCoroutine(AnimateCoinScore(score));
        }
    }

    private IEnumerator AnimateCoinScore(int score)
    {
        // Reset and show coin score text
        coinScoreText.gameObject.SetActive(true);
        coinScoreText.text = $"+{score}";
        coinScoreText.color = new Color(coinScoreText.color.r, coinScoreText.color.g, coinScoreText.color.b, 1f);
        Vector3 startPosition = coinScoreText.transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < coinScoreDisplayDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / coinScoreDisplayDuration;

            // Move upward
            coinScoreText.transform.position = startPosition + Vector3.up * (coinScoreFloatSpeed * normalizedTime);

            // Fade out using animation curve
            float alpha = coinScoreAlphaCurve.Evaluate(normalizedTime);
            coinScoreText.color = new Color(coinScoreText.color.r, coinScoreText.color.g, coinScoreText.color.b, alpha);

            yield return null;
        }

        // Hide and reset position
        coinScoreText.gameObject.SetActive(false);
        coinScoreText.transform.position = startPosition;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (scoringSystem != null)
        {
            scoringSystem.onScoreChanged.RemoveListener(UpdateScore);
            scoringSystem.onComboMultiplierChanged.RemoveListener(UpdateCombo);
            scoringSystem.onCoinCollected.RemoveListener(ShowCoinScore);
        }

        if (livesSystem != null)
        {
            livesSystem.onLivesChanged.RemoveListener(UpdateLives);
        }
    }
} 