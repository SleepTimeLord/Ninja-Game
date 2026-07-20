using System;
using UnityEngine;

public class SlashTrigger : MonoBehaviour
{
    public CharacterController cc;
    /// <summary>
    /// Invoked once the player hits an enemy
    /// </summary>
    public event Action<Enemy> EnemyHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy confirmedEnemy = collision.GetComponentInParent<Enemy>();

            this.EnemyHit.Invoke(confirmedEnemy);

            cc.ctx.enemyKillCombo++;
        }
    }
}
