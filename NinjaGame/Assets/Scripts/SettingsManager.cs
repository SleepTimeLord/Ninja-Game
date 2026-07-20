using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    public static bool isPaused = false;
    public bool inGameScene = false;
    public GameObject settingMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        // start out with pause disabled
        Resume();

        if (inGameScene)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true; 
            Cursor.lockState = CursorLockMode.None;
        }
    } 

    public void OnPause(InputAction.CallbackContext context)
    {
        if (isPaused && context.performed)
        {
            Resume();
        }
        else if (!isPaused && context.performed)
        {
            Pause();
        }
    }

    public void Resume()
    {
        settingMenuUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
        if (inGameScene)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

    public void Pause()
    {
        settingMenuUI.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        Cursor.visible = true; 
        Cursor.lockState = CursorLockMode.None;
    }
}
