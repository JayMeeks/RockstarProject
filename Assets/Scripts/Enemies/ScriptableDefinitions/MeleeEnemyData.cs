using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyData", menuName = "Scriptable Objects/Enemies/MeleeEnemyData")]
public class MeleeEnemyData : EnemyData
{
    public int damage;
    public float attackRange;
    public float attackCooldown;
}
