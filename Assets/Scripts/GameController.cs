using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    #region Singleton

    private static GameController _gameControllerInstance;

    public static GameController Instance
    {
        get
        {
            if (_gameControllerInstance == null) _gameControllerInstance = FindObjectOfType<GameController>();
            return _gameControllerInstance;
        }
    }

    #endregion

    public GameState State { get; private set; } = GameState.PreGame;
    public const float TargetFrameRate = 60f;

    [Header("UI References")]
    [SerializeField] private Canvas mainUI;

    [SerializeField] private GameObject startGameDisplay;
    private TMP_Text _startGameText;
    private Timer _startGameTimer;

    [SerializeField] private GameObject endGameDisplay;
    [SerializeField] private GameObject endGameStars;
    private Image[] _stars;
    private TMP_Text _endGameText;
    private Timer _endGameTimer;

    [SerializeField] private TMP_Text gameCountdownText;

    [Header("Stats")]
    [SerializeField] private float startGameCountdown;
    [SerializeField] private float endGameCountdown;

    private Truck _truck;
    private float _maxValue;

    private InputManager _inputManager;

    #region Unity Event

    private void OnEnable()
    {
        _inputManager = new InputManager();

        // Handle game pause input
        _inputManager.Game.Escape.performed += EscapeOnPerformed;

        _inputManager.Enable();
    }

    private void OnDisable()
    {
        _inputManager?.Disable();
    }

    private void Awake()
    {
        _startGameText = startGameDisplay.GetComponentInChildren<TMP_Text>();
        _endGameText = endGameDisplay.GetComponentInChildren<TMP_Text>();
        _stars = endGameStars.GetComponentsInChildren<Image>();

        _truck = FindObjectOfType<Truck>();
        foreach (var item in FindObjectsOfType<Item>()) _maxValue += item.value;
    }

    private void Start()
    {
        // Initial game setup
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = (int)TargetFrameRate;

        PostProcessingController.Instance.SetDepthOfField(true);
        PostProcessingController.Instance.SetChromaticAberration(false);
        PostProcessingController.Instance.SetVignetteIntensity(PostProcessingController.DefaultVignetteIntensity);

        mainUI.gameObject.SetActive(true);
        SetCursorEnabled(false);
        SetTimeScale(0f);

        startGameDisplay.SetActive(true);
        endGameDisplay.SetActive(false);

        _startGameTimer = new Timer(startGameCountdown);
        _endGameTimer = new Timer(endGameCountdown);
    }

    private void Update()
    {
        if (State == GameState.PreGame)
        {
            if (_startGameTimer.IsReachedUnscaled()) StartGame();
            _startGameText.SetText($"Get ready!\nStart packing in {((int)_startGameTimer.Progress + 1).ToString()} seconds!");
        }

        if (State == GameState.InProgress)
        {
            if (_endGameTimer.IsReached()) EndGame();
            gameCountdownText.SetText(((int)_endGameTimer.Progress + 1).ToString());
        }
        else gameCountdownText.SetText("");
    }

    #endregion

    #region Input Handlers

    private void EscapeOnPerformed(InputAction.CallbackContext context)
    {
        InputTypeController.Instance.CheckInputType(context);
    }

    #endregion

    public void SetGameState(GameState state)
    {
        State = state;
    }

    private static void SetCursorEnabled(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;
    }

    public static void SetTimeScale(float scale = 1f)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.01666667f * Time.timeScale;
    }

    #region Slow Motion Methods

    private static IEnumerator SlowMotionEffect(float scale, float duration)
    {
        // Slow down
        SetTimeScale(scale);
        PostProcessingController.Instance.SetChromaticAberration(true);
        PostProcessingController.Instance.SetVignetteIntensity(PostProcessingController.DefaultVignetteIntensity + 0.1f);

        yield return new WaitForSeconds(duration);

        // Back to normal
        SetTimeScale();
        PostProcessingController.Instance.SetChromaticAberration(false);
        PostProcessingController.Instance.SetVignetteIntensity(PostProcessingController.DefaultVignetteIntensity);
    }

    public void PlaySlowMotionEffect(float scale = 0.5f, float duration = 0.25f)
    {
        StartCoroutine(SlowMotionEffect(scale, duration));
    }

    #endregion

    #region Start & End Game Methods

    private void StartGame()
    {
        // Update game state
        State = GameState.InProgress;
        SetTimeScale(1f);

        // Update UI
        startGameDisplay.SetActive(false);
        PostProcessingController.Instance.SetDepthOfField(false);
    }

    private void EndGame()
    {
        // Update game state
        State = GameState.EndGame;
        SetTimeScale(0f);
        SetCursorEnabled(true);

        // Update UI
        _endGameText.SetText($"Time's up!\n\nItems packed: {_truck.ItemsCount}\nTotal value: ${_truck.TotalValue}");
        endGameDisplay.SetActive(true);
        PostProcessingController.Instance.SetDepthOfField(true);

        // Update ranking
        RankLevel();
    }
    
    #endregion

    private void RankLevel()
    {
        var score = _truck.TotalValue / _maxValue;
        for (var i = 0; i < 5; i++)
        {
            _stars[i].gameObject.SetActive(score >= (i / 5f));
        }
    }
}