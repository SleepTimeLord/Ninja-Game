using UnityEngine;
using System;

public class JTrashcanTrigger : MonoBehaviour, ICharacterInteractable
{
    public event Action OnPlayerHide;

    // put player in
    public CharacterController cc;

    public void Interact()
    {
        cc.HandleHiding();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (cc == null) return;

            cc.ctx.nearestInteractable = this.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cc.ctx.nearestInteractable = null;
        }
    }
}
