using UnityEngine;

/// <summary>
/// Just runtime script so buttons dont have to 
/// rely on scenecontroller not getting destroyed
/// </summary>
public class GoToLevel : MonoBehaviour
{
    public void GoToNextLevel()
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.NextLevel();
        }
    }

    public void LoadScene(string scene)
    {
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene(scene);
        }
    }
}
