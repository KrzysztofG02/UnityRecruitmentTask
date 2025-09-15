using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

[Serializable]
public class InfoPopup
{
    public GameObject popupObject;
    public Button okButton;
}

[Serializable]
public class OpenPopup
{
    public GameObject popupObject;
    public Button yesButton;
    public Button noButton;
}

[Serializable]
public class TakePopup
{
    public GameObject popupObject;
    public Button yesButton;
    public Button noButton;
}

[Serializable]
public class GameOverScreen
{
    public GameObject screenObject;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public Button tryAgainButton;
}

[Serializable]
public class MainMenu
{
    public GameObject mainMenuScreen;
    public Button startButton;
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;

    [SerializeField] private GameOverScreen gameOverScreen;

    [Header("Reset Settings")]
    [SerializeField] private float gameOverDelay = 1f;

    [Header("Popups Settings")]
    [SerializeField] private float popupShowDelay = 0.03f;
    [SerializeField] private InfoPopup keyRequiredPopup;
    [SerializeField] private OpenPopup openPopup;
    [SerializeField] private TakePopup takePopup;

    [Header("Info Message")]
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private float messageDuration = 1f;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    private Coroutine messageCoroutine;

    private PlayerController playerController;
    private GameManager gameManager;

    private void Awake()
    {
        this.playerController = FindFirstObjectByType<PlayerController>();
        this.gameManager = FindFirstObjectByType<GameManager>().GetComponent<GameManager>();

        this.keyRequiredPopup.popupObject.SetActive(false);
        this.openPopup.popupObject.SetActive(false);
        this.takePopup.popupObject.SetActive(false);

        this.gameOverScreen.screenObject.SetActive(false);

        this.mainMenu.mainMenuScreen.SetActive(true);

        this.ToggleTimerVisibility(false);

        this.mainMenu.startButton.onClick.AddListener(() =>
        {
            this.gameManager.SetGame();

            this.mainMenu.mainMenuScreen.SetActive(false);
        });

        this.keyRequiredPopup.okButton.onClick.AddListener(() => { 
            this.CloseKeyRequiedPopup();
            this.TogglePlayerControl(true); 
        });
        this.openPopup.noButton.onClick.AddListener(() => { 
            this.CloseOpenPopup();
            this.TogglePlayerControl(true); 
        }) ;
        this.takePopup.noButton.onClick.AddListener(() => {
            this.CloseTakePopup();
            this.TogglePlayerControl(true);
        });

        this.gameOverScreen.tryAgainButton.onClick.AddListener(() => {
            gameManager.ResetGame();

            this.CloseGameOverScreen();
            this.TogglePlayerControl(true);
        });
    }

    public void Update()
    {
        this.timerText.text = gameManager.gameTime + " s";
    }

    public void ToggleTimerVisibility(bool isVisible)
    {
        if(isVisible)
        {
            this.timerText.alpha = 1f;
        }
        else
        {
            this.timerText.alpha = 0f;
        }
    }

    private void TogglePlayerControl(bool enable)
    {
        if (!enable)
        {
            this.playerController.StopMoving();
        }
        this.playerController.enabled = enable;
    }

    public void ShowKeyRequiredPopup()
    {
        StartCoroutine(ShowPopup());

        IEnumerator ShowPopup()
        {
            yield return new WaitForSeconds(popupShowDelay);

            this.keyRequiredPopup.popupObject.SetActive(true);
            TogglePlayerControl(false);
        }
    }

    private void CloseKeyRequiedPopup()
    {
        this.keyRequiredPopup.popupObject.SetActive(false);
        this.TogglePlayerControl(true);
    }


    public void ShowOpenPopup(Action onYes)
    {
        StartCoroutine(ShowPopup());

        IEnumerator ShowPopup()
        {
            yield return new WaitForSeconds(popupShowDelay);

            this.openPopup.popupObject.SetActive(true);
            this.TogglePlayerControl(false);

            this.openPopup.yesButton.onClick.RemoveAllListeners();
            this.openPopup.yesButton.onClick.AddListener(() =>
            {
                onYes?.Invoke();
                this.CloseOpenPopup();
                this.TogglePlayerControl(true);
            });
        }
    }

    private void CloseOpenPopup()
    {
        this.openPopup.popupObject.SetActive(false);
    }


    public void ShowTakePopup(Action onYes)
    {
        StartCoroutine(ShowPopup());

        IEnumerator ShowPopup()
        {
            yield return new WaitForSeconds(popupShowDelay);

            this.takePopup.popupObject.SetActive(true);
            TogglePlayerControl(false);

            this.takePopup.yesButton.onClick.RemoveAllListeners();
            this.takePopup.yesButton.onClick.AddListener(() =>
            {
                onYes?.Invoke();
                CloseTakePopup();
                TogglePlayerControl(true);
            });
        }
    }

    private void CloseTakePopup()
    {
        this.takePopup.popupObject.SetActive(false);
    }


    public void ShowMessage(string message)
    {
        if (this.infoText != null)
        {
            if (this.messageCoroutine != null)
            {
                StopCoroutine(this.messageCoroutine);
            }

            this.messageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
        }
    }

    private IEnumerator ShowMessageCoroutine(string message)
    {
        this.infoText.text = message;
        this.infoText.alpha = 1f;
        this.infoText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        float t = 0f;
        while (t < this.fadeDuration)
        {
            t += Time.deltaTime;
            this.infoText.alpha = Mathf.Lerp(1f, 0f, t / this.fadeDuration);
            yield return null;
        }

        this.infoText.alpha = 0f;
        this.infoText.gameObject.SetActive(false);
        this.messageCoroutine = null;
    }

    public void FinishGame()
    {
        this.gameManager.FinishGame();
        this.ShowGameOverScreen();
    }

    private void ShowGameOverScreen()
    {
        StartCoroutine(DelayedShowGameOverScreenCoroutine());
        
        IEnumerator DelayedShowGameOverScreenCoroutine()
        {
            yield return new WaitForSeconds(this.gameOverDelay);
            this.gameOverScreen.screenObject.SetActive(true);
            this.TogglePlayerControl(false);
            this.ToggleTimerVisibility(false);

            this.gameOverScreen.currentScoreText.text = "Score: " + this.gameManager.gameTime + " s";
            this.gameOverScreen.highScoreText.text = "Highscore: " + this.gameManager.bestGameTime + " s";
        }
    }

    private void CloseGameOverScreen()
    {
        this.gameOverScreen.screenObject.SetActive(false);
    }

    public void ClearOverlay()
    {
        this.ToggleTimerVisibility(false);
        this.CloseAllPopups();
    }

    private void CloseAllPopups()
    {
        this.keyRequiredPopup.popupObject.SetActive(false);
        this.openPopup.popupObject.SetActive(false);
        this.takePopup.popupObject.SetActive(false);
    }
}
