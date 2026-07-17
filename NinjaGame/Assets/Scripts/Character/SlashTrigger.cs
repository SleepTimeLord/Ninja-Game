using UnityEngine;

public class SlashTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // put the enemy death trigger here!!!!
            Debug.Log("enemy dead");
        }
    }
}
