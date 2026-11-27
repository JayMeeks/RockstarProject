using UnityEngine;

[CreateAssetMenu(fileName = "BaseEnemyStats", menuName = "Scriptable Objects/EnemyStats/BaseStats")]
public class BaseEnemyStats : ScriptableObject
{
    [Header("Base enemy stats")]
    
    [Tooltip("Enemy maximum and starting health")]
    [SerializeField] public int maxHealth;
    
    [Tooltip("Enemy move speed")]
    [SerializeField] public float moveSpeed;
    
    [Tooltip("How much health should the enemy drop when killed")]
    [SerializeField] public int healthOnKill;
}
