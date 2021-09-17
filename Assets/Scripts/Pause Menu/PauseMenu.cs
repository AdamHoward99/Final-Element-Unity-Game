using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public Text gameText;
    public Text keyboardText;
    public Text controllerText;
    public Button resumeText;
    public Button controlsText;
    public Button menuText;
    public Button backText;
    public Button backText2;
    public Button keyboardControls;
    public Button controllerControlsButton;

    private void Awake() => Time.timeScale = 1f;

    // Update is called once per frame
    void Update()
    {
        if(DetectPauseInput())
            GameIsPaused = Toggle(!GameIsPaused);
    }

    private bool DetectPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Joystick1Button9) || Input.GetKeyDown(KeyCode.Joystick2Button9))
            return true;

        return false;
    }

    private bool Toggle(bool value = false)
    {
        Time.timeScale = Convert.ToSingle(!value);
        pauseMenuUI.SetActive(value);
        return value;
    }

    public void Resume() => Toggle();

    public void ShowControlMenu()
    {
        HideMainPauseMenu();
        gameText.gameObject.SetActive(true);
        keyboardControls.gameObject.SetActive(true);
        controllerControlsButton.gameObject.SetActive(true);
        backText.gameObject.SetActive(true);
    }

    private void HideControlMenu()
    {
        keyboardControls.gameObject.SetActive(false);
        controllerControlsButton.gameObject.SetActive(false);
        backText.gameObject.SetActive(false);
    }

    private void HideMainPauseMenu()
    {
        resumeText.gameObject.SetActive(false);
        controlsText.gameObject.SetActive(false);
        menuText.gameObject.SetActive(false);
    }

    public void ShowMainPauseMenu()
    {
        HideControlMenu();
        resumeText.gameObject.SetActive(true);
        controlsText.gameObject.SetActive(true);
        menuText.gameObject.SetActive(true);
    }

    private void HideKeyboardControls()
    {
        keyboardText.gameObject.SetActive(false);
        controllerText.gameObject.SetActive(false);
        backText2.gameObject.SetActive(false);
    }

    public void ShowKeyboardControls()
    {
        HideControlMenu();
        keyboardText.gameObject.SetActive(true);
        backText2.gameObject.SetActive(true);
        gameText.gameObject.SetActive(false);
    }

    private void HideControllerControls()
    {
        controllerText.gameObject.SetActive(true);
        backText2.gameObject.SetActive(false);
        gameText.gameObject.SetActive(true);
    }

    public void ShowControllerControls()
    {
        HideControlMenu();
        controllerText.gameObject.SetActive(true);
        backText2.gameObject.SetActive(true);
        gameText.gameObject.SetActive(false);
    }

    public void BackToControlMenu()
    {
        HideKeyboardControls();
        ShowControlMenu();
    }
}


