using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject settingMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        // start out with pause disabled
        Resume();
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
    }

    public void Pause()
    {
        settingMenuUI.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
    }
}
