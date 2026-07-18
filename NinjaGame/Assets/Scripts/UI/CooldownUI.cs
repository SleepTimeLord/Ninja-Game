using UnityEngine;
using UnityEngine.UI;

public enum AbilityType
{
    Dash,
    SneakAttack
}

public class CooldownUI : MonoBehaviour
{
    [Header("References")]
    public CharacterController cc;
    public AbilityType abilityType;
    public Image imageCooldown;
    void Start()
    {
        imageCooldown.fillAmount = 0;
    }
    void Update()
    {
        PlayerContext ctx = cc.ctx;
        
        float nextTimeReady = 0f;
        float maxCooldown = 1f;

        switch (abilityType)
        {
            case AbilityType.Dash:
                nextTimeReady = ctx.nextTimeReady;
                maxCooldown = ctx.dashCooldown; 
                break;
            
            case AbilityType.SneakAttack:
                nextTimeReady = ctx.attackNextTimeReady;
                maxCooldown = ctx.sneakAttackCooldown;
                break;
        }

        // calculate time remaining
        float remainingTime = nextTimeReady - Time.time;

        if (remainingTime > 0)
        {
            imageCooldown.fillAmount = remainingTime / maxCooldown;
        }
        else
        {
            imageCooldown.fillAmount = 0;
        }
    }
}
