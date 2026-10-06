using UnityEngine;

public class AttackManager : MonoBehaviour
{
  public void HammerAttack()
  {
    PlayerController.Instance.CreateWeaponHitbox();
  }
}
