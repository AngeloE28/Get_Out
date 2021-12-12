using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private PlayerMovement player;

    [Header("GUI")]
    [SerializeField] private GameObject escapeTitle;
    [SerializeField] private GameObject backGroundPanel;
    private GameObject currentBtn;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject pauseFirstBtn;
    [SerializeField] private GameObject backgroundPanel;
    private float mouseInactiveTimer = 1.0f;


    [Header("GameOverUI")]
    [SerializeField] private ExitPortal exitPortal;
    [SerializeField] private GameObject gameOverWindow;
    [SerializeField] private GameObject GameOverText;
    [SerializeField] private List<GameObject> gameOverBtns;
    [SerializeField] private Color fadeInColour;
    [SerializeField] private Color ogBackgroundColour;
    private Image _gameOverBackground;

    [Header("Portal Blocker")]
    [SerializeField] private List<GameObject> portalBlockers;

    [Header("Blue Room")]
    [SerializeField] private Button blueRoomBtn;
    [SerializeField] private List<Renderer> blueSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newEBlueColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogBlueColor;

    [Header("Red Room")]
    [SerializeField] private Button redRoomBtn;
    [SerializeField] private List<Renderer> redSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newERedColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogRedColor;

    [Header("Green Room")]
    [SerializeField] private PadManager greenRoomPads;
    [SerializeField] private List<Renderer> greenSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newEGreenColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogGreenColor;

    [Header("Orange Room")]
    [SerializeField] private PadManager orangeRoomPads;
    [SerializeField] private List<Renderer> orangeSignRenderers;
    [ColorUsage(true, true)]
    [SerializeField] private Color newEOrangeColor;
    [ColorUsage(true, true)]
    [SerializeField] private Color ogOrangeColor;

    private bool _allRoomClear = false;
    private bool _isGamePaused = false;
    private bool _isGameRunning = true;

    private void Awake()
    {
        Time.timeScale = 1.0f;

        _allRoomClear = false;
        _isGamePaused = false;
        _isGameRunning = true;

        currentBtn = pauseFirstBtn;

        InitializeSigns();

        _gameOverBackground = gameOverWindow.GetComponent<Image>();
        _gameOverBackground.color = ogBackgroundColour;
        foreach (var btn in gameOverBtns)
        {
            if (btn.activeSelf)
                btn.SetActive(false);
        }

        escapeTitle.SetActive(true);

        if (backGroundPanel.activeSelf)
            backGroundPanel.SetActive(false);
        if (GameOverText.activeSelf)
            GameOverText.SetActive(false);
        if (gameOverWindow.activeSelf)
            gameOverWindow.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        player.GetPlayerInputActions().Player.Pause.performed += _ => GamePauseState();
        float countDown = 3.0f;
        Invoke(nameof(DisableEscapeText), countDown);
    }

    // Update is called once per frame
    void Update()
    {        
        ShowGameOver();
        CursorManager();
        InputManagerPaused();
        InputManagerGameOver();
    }

    private void LateUpdate()
    {
        GameOverCondition();
    }

    private void DisableEscapeText()
    {
        escapeTitle.SetActive(false);
    }

    private void InitializeSigns()
    {
        foreach (var pb in portalBlockers)
        {
            if (!pb.activeSelf)
                pb.SetActive(true);
        }

        foreach (var bsr in blueSignRenderers)
            bsr.material.SetColor("_EmissionColor", ogBlueColor);

        foreach (var rsr in redSignRenderers)
            rsr.material.SetColor("_EmissionColor", ogRedColor);

        foreach (var gsr in greenSignRenderers)
            gsr.material.SetColor("_EmissionColor", ogGreenColor);

        foreach (var osr in orangeSignRenderers)
            osr.material.SetColor("_EmissionColor", ogOrangeColor);
    }

    private void GameOverCondition()
    {
        if (_allRoomClear)
        {
            foreach (var pb in portalBlockers)
                pb.SetActive(false);
        }

        if (blueRoomBtn.BtnPressed() && redRoomBtn.BtnPressed() &&
            greenRoomPads.GetRoomClear() && orangeRoomPads.GetRoomClear())
        {
            _allRoomClear = true;
        }
        else
            _allRoomClear = false;

        if (blueRoomBtn.BtnPressed())
        {
            foreach (var bsr in blueSignRenderers)
                bsr.material.SetColor("_EmissionColor", newEBlueColor);
        }

        if (redRoomBtn.BtnPressed())
        {
            foreach (var rsr in redSignRenderers)
                rsr.material.SetColor("_EmissionColor", newERedColor);
        }

        if (greenRoomPads.GetRoomClear())
        {
            foreach (var gsr in greenSignRenderers)
                gsr.material.SetColor("_EmissionColor", newEGreenColor);
        }

        if (orangeRoomPads.GetRoomClear())
        {
            foreach (var osr in orangeSignRenderers)
                osr.material.SetColor("_EmissionColor", newEOrangeColor);
        }
    }

    private void ShowGameOver()
    {
        if(_allRoomClear && exitPortal.PlayerLeft())
        {
            _isGameRunning = false;
            float smoothVal = 8.0f;
            gameOverWindow.SetActive(true);

            if(_gameOverBackground.color == fadeInColour)
            {
                GameOverText.SetActive(true);
                foreach(var btn in gameOverBtns)
                {
                    btn.SetActive(true);
                }
            }
            _gameOverBackground.color = Color.Lerp(_gameOverBackground.color, fadeInColour, smoothVal * Time.deltaTime);
        }
    }

    private void InputManagerGameOver()
    {
        currentBtn = gameOverBtns[1];
        Vector2 gamePadNavigation = player.GetPlayerInputActions().UI.Navigate.ReadValue<Vector2>();
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (gamePadNavigation != Vector2.zero)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(currentBtn);

                Cursor.visible = false;
            }
        }
    }

    private void CursorManager()
    {
        if (_isGamePaused && _isGameRunning)
            ShowMouseCursor();
        if (!_isGamePaused && _isGameRunning)
            Cursor.visible = false;
        if (!_isGameRunning)
            ShowMouseCursor();
    }

    private void ShowMouseCursor()
    {        
        Vector2 mousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (mousePos != Vector2.zero)
        {
            Cursor.visible = true;
            mouseInactiveTimer = 1.0f;
        }
        else
            HideMouseCursor();
    }

    private void HideMouseCursor()
    {
        if (mouseInactiveTimer > 0)
            mouseInactiveTimer -= Time.unscaledDeltaTime;
        else
            Cursor.visible = false;
    }

    private void InputManagerPaused()
    {
        if(_isGamePaused)
        {
            if (pauseUI.activeSelf)
                currentBtn = pauseFirstBtn;

            Vector2 gamePadNavigation = player.GetPlayerInputActions().UI.Navigate.ReadValue<Vector2>();
            if(EventSystem.current.currentSelectedGameObject == null)
            {
                if(gamePadNavigation != Vector2.zero)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(currentBtn);

                    Cursor.visible = false;
                }
            }
                
        }
    }

    private void GamePauseState()
    {
        if (_isGamePaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        if (pauseUI.activeSelf)
            pauseUI.SetActive(false);
        if (backgroundPanel.activeSelf)
            backgroundPanel.SetActive(false);

        currentBtn = pauseFirstBtn;

        Time.timeScale = 1.0f;

        _isGamePaused = false;
    }

    private void Pause()
    {
        pauseUI.SetActive(true);
        backgroundPanel.SetActive(true);
        currentBtn = pauseFirstBtn;

        Time.timeScale = 0.0f;

        _isGamePaused = true;
    }

    public void Restart()
    {
        sceneLoader.LoadScene(Scene.Game);        
    }

    public void Quit()
    {
        Time.timeScale = 1.0f;

        sceneLoader.LoadScene(Scene.MainMenu);
    }

}
