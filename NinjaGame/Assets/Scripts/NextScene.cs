using UnityEngine;

public class NextScene : MonoBehaviour
{
    public string nextScene;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneController.Instance.LoadScene(nextScene);
        }
    }
}
