using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("씬 설정")]
    public string gameSceneName = "GameScene";

    [Header("조작방법 패널")]
    public GameObject howToPlayPanel;
    public CanvasGroup howToPlayCanvasGroup;

    [Header("페이드 설정")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.2f;

    [Header("버튼 애니메이션")]
    public RectTransform startButtonRect;    // StartButton 연결
    public RectTransform howToPlayButtonRect; // HowToPlayButton 연결
    public float pulseScale = 1.05f;
    public float pulseDuration = 0.8f;

    void Start()
    {
        // 패널 숨기기
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
            if (howToPlayCanvasGroup != null)
                howToPlayCanvasGroup.alpha = 0f;
        }

        // StartButton 펄스 애니메이션
        if (startButtonRect != null)
        {
            startButtonRect.DOScale(pulseScale, pulseDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        // HowToPlayButton 펄스 애니메이션 (약간 딜레이)
        if (howToPlayButtonRect != null)
        {
            howToPlayButtonRect.DOScale(pulseScale, pulseDuration)
                .SetDelay(0.4f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    // GAME START 버튼
    public void OnStartButton()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    // 조작방법 버튼
    public void OnHowToPlayButton()
    {
        if (howToPlayPanel == null) return;

        howToPlayPanel.SetActive(true);

        if (howToPlayCanvasGroup != null)
        {
            howToPlayCanvasGroup.alpha = 0f;
            howToPlayCanvasGroup.DOFade(1f, fadeInDuration);
        }
    }

    // 닫기 버튼
    public void OnCloseButton()
    {
        if (howToPlayCanvasGroup != null)
        {
            howToPlayCanvasGroup.DOFade(0f, fadeOutDuration)
                .OnComplete(() => howToPlayPanel.SetActive(false));
        }
        else
        {
            howToPlayPanel.SetActive(false);
        }
    }
}