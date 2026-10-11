using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{ 
    [SerializeField]
    protected int maxDurability, currentDurability;
    [SerializeField]
    protected int lightAttackDamage, heavyAttackDamage;
    [SerializeField]
    protected int playerHpGainOnHit;
    [SerializeField]
     protected int heavyAttackHpCost;


    protected abstract void OnUseLightAttack();
    protected abstract void OnUseHeavyAttack();

    protected abstract void OnWeaponBreak();
}
