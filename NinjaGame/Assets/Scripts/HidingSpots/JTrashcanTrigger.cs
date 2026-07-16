using UnityEngine;
using UnityEngine.TextCore.Text;

public class JTrashcanTrigger : MonoBehaviour, ICharacterInteractable
{
    // put player in
    public CharacterController cc;
    public void Interact()
    {
        
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
        cc.ctx.nearestInteractable = null;
    }
}
