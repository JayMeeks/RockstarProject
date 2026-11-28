using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private static bool _isPaused = false;
    private static event Action<bool> OnPausedChanged;
    private InputAction _pauseAction = new InputAction("Pause", InputActionType.Button);
    
    private PlayerInput _playerInput;
    private PlayerInput.PlayerDefaultActions _playerDefaultActions;
        
    private string _gameplayMap = "Player";
    private string _uiMap = "UI";

    [SerializeField]private GameObject _menuRoot;
    [SerializeField]private GameObject _pausePanel;
    [SerializeField]private GameObject _optionsPanel;

    private string _mainMenuScene = "MainMenu";

    private CursorLockMode _previousLockState;
    private bool _previousCursorVisible;

    public static bool GetIsPaused()
    {
        return _isPaused;
    }
    
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerDefaultActions = _playerInput.PlayerDefault;
        
        if (_pauseAction.bindings.Count == 0)
        {
            AddDefaultBindings();
        }

        _isPaused = false;
        Time.timeScale = 1f;

        if (_menuRoot != null) _menuRoot.SetActive(false);


    }

    private void AddDefaultBindings()
    {
        _pauseAction.AddBinding("<Keyboard>/p");
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _pauseAction.performed += OnPausePerformed;
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _pauseAction.performed -= OnPausePerformed;
        _pauseAction.Disable();
    }

    private void OnDestroy()
    {
        if (_isPaused) ApplyResumeState();
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx) => TogglePause();

    public void TogglePause()
    {
        if (!_isPaused) Pause();
        else GoBack();
    }

    public void Pause()
    {
        if (_isPaused) return;
        
        _isPaused = true;
        Time.timeScale = 0f;

        _previousLockState = Cursor.lockState;
        _previousCursorVisible = Cursor.visible;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Mouse.current != null)
        {
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        }

        if (_menuRoot != null)
        {
            _menuRoot.SetActive(true);
        }

        ShowMain();

        OnPausedChanged?.Invoke(true);
    }

    public void Resume()
    {
        if (!_isPaused) return;
        ApplyResumeState();
        OnPausedChanged?.Invoke(false);
    }

    public void ShowMain()
    {
        if (_pausePanel != null)
        {
            _pausePanel.SetActive(true);
        }

        if (_optionsPanel != null)
        {
            _optionsPanel.SetActive(false);
        }
    }

    public void ShowOptions()
    {
        if (_pausePanel != null)
        {
            _pausePanel.SetActive(false);
        }

        if (_optionsPanel != null)
        {
            _optionsPanel.SetActive(true);
        }
    }

    public void QuitToMainMenu()
    {
        ApplyResumeState();
        OnPausedChanged?.Invoke(false);
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void GoBack()
    {
        if (_optionsPanel != null && _optionsPanel.activeSelf) ShowMain();
        else Resume();
    }

    private void ApplyResumeState()
    {
        _isPaused = false;
        
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = _previousCursorVisible;
        
        if (_menuRoot != null) _menuRoot.SetActive(false);
    }
    
    
// Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
}
