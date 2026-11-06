using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{

    protected int currentDurability;
    protected bool weaponEnabled;
    
    protected abstract void OnUseLightAttack();
    protected abstract void OnUseHeavyAttack();
    protected abstract void OnUseWeaponSpecial();
    protected abstract void OnWeaponBreak();

    public void SetWeaponEnabled(bool enabled)
    {
        this.weaponEnabled = enabled;
    }
}
