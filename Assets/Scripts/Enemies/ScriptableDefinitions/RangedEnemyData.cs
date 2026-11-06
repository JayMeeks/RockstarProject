using Enemies;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedEnemyData", menuName = "Scriptable Objects/Enemies/RangedEnemyData")]
public class RangedEnemyData : EnemyData
{
    public int damage;
    public float attackRange;
    public float attackCooldown;

    public Bullet projectile;
    public float projectileSpeed;

    public float retreatDistance;

}
