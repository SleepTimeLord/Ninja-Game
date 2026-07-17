using UnityEngine;

public class SlashTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("enemy dead");
        }
    }
}
