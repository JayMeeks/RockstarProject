using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyData", menuName = "Scriptable Objects/Enemies/MeleeEnemyData")]
public class MeleeEnemyData : EnemyData
{
    public float damage;
    public float attackRange;
    public float attackCooldown;
}
