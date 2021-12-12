using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    [Header("MainMenu")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject controlsButton;

    [Header("Controls")]
    [SerializeField] private GameObject controlsUI;
    [SerializeField] private GameObject controlsFirstButton;

    private PlayerInputActions inputActions;
    private float mouseInactiveTimer = 1.0f;
    private GameObject currentBtn;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        Time.timeScale = 1.0f;
        currentBtn = playButton;

        mainMenuUI.SetActive(true);
        controlsUI.SetActive(false);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
    }

    private void InputManager()
    {
        Vector2 gamePadNavigation = inputActions.UI.Navigate.ReadValue<Vector2>();
        if(EventSystem.current.currentSelectedGameObject == null)
        {
            if(gamePadNavigation != Vector2.zero)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(currentBtn);

                Cursor.visible = false;
            }
        }

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

    public void Play()
    {
        currentBtn = playButton;

        sceneLoader.LoadScene(Scene.Game);
    }

    public void ShowControls()
    {
        mainMenuUI.SetActive(false);
        controlsUI.SetActive(true);

        currentBtn = controlsFirstButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void CloseControls()
    {
        controlsUI.SetActive(false);
        mainMenuUI.SetActive(true);

        currentBtn = controlsButton;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(currentBtn);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
